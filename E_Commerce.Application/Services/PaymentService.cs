using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Payments;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Payments;
using E_Commerce.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Application.Services
{
    internal class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IPaymentGateway paymentGateway;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IPaymentGateway paymentGateway)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.paymentGateway = paymentGateway;
        }

        public async Task<Result<CreatePaymentResponse>> CreatePaymentAsync(string buyerEmail, CreatePaymentRequest request, CancellationToken ct = default)
        {
            var orderRepository = unitOfWork.GetRepository<Order, Guid>();
            var paymentRepository = unitOfWork.GetRepository<Payment, Guid>();

            var specification = new OrderForPaymentSpecification(request.OrderId, buyerEmail);

            var order = await orderRepository.GetByIdAsync(specification, ct);

            if (order is null)
            {
                return Result<CreatePaymentResponse>.Fail(Error.NotFound("Order.Failure", $"Order with id '{request.OrderId}' was not found."));
            }

            if (order.Payments.Any(p => p.Status == PaymentStatus.Succeeded))
            {
                return Result<CreatePaymentResponse>.Fail(Error.Conflict("Payment.AlreadyPaid", "This order has already been paid."));
            }

            var amount = order.GetTotal();

            // If a pending payment intent already exists for this order, just update its amount instead of creating a new one
            var pendingPayment = order.Payments
                .Where(p => p.Status == PaymentStatus.Pending && !string.IsNullOrEmpty(p.PaymentIntentId))
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefault();

            CreatePaymentResponse gatewayResponse;

            if (pendingPayment is not null)
            {
                try
                {
                    gatewayResponse = await paymentGateway.UpdatePaymentIntentAsync(pendingPayment.PaymentIntentId!, amount, ct);

                    pendingPayment.Amount = amount;
                    paymentRepository.Update(pendingPayment);
                }
                catch (Exception)
                {
                    // The stored PaymentIntent is no longer valid on Stripe's side (expired, deleted, or created under a different key).
                    // Fall back to creating a brand new one instead of failing the whole request.
                    gatewayResponse = await paymentGateway.CreatePaymentIntentAsync(amount, $"create-payment-{order.Id}", ct);

                    pendingPayment.Amount = amount;
                    pendingPayment.PaymentIntentId = gatewayResponse.PaymentIntentId;
                    paymentRepository.Update(pendingPayment);
                }
            }
            else
            {
                gatewayResponse = await paymentGateway.CreatePaymentIntentAsync(amount, $"create-payment-{order.Id}", ct);

                var payment = new Payment
                {
                    OrderId = order.Id,
                    Amount = amount,
                    Status = PaymentStatus.Pending,
                    PaymentIntentId = gatewayResponse.PaymentIntentId
                };

                paymentRepository.Add(payment);
            }

            await unitOfWork.SaveChangesAsync(ct);

            return Result<CreatePaymentResponse>.Ok(gatewayResponse);
        }

        public async Task<Result> HandleStripeWebhookAsync(string json, string stripeSignature, CancellationToken ct = default)
        {
            PaymentWebhookEvent webhookEvent;

            try
            {
                webhookEvent = paymentGateway.ParseWebhookEvent(json, stripeSignature);
            }
            catch (Exception)
            {
                return Result.Fail(Error.Validation("Payment.InvalidWebhook", "Stripe webhook signature verification failed."));
            }

            if (webhookEvent.EventType == PaymentWebhookEventType.Other || string.IsNullOrEmpty(webhookEvent.PaymentIntentId))
            {
                return Result.OK();
            }

            var processedEventRepository = unitOfWork.GetRepository<ProcessedWebhookEvent, Guid>();

            var alreadyProcessedSpec = new ProcessedWebhookEventByStripeIdSpecification(webhookEvent.StripeEventId);
            var alreadyProcessedCount = await processedEventRepository.CountAsync(alreadyProcessedSpec, ct);

            if (alreadyProcessedCount > 0)
            {
                // Stripe already sent this exact event before (retry/duplicate delivery) - nothing to do.
                return Result.OK();
            }

            var paymentRepository = unitOfWork.GetRepository<Payment, Guid>();
            var orderRepository = unitOfWork.GetRepository<Order, Guid>();

            var spec = new PaymentByIntentIdSpecification(webhookEvent.PaymentIntentId);
            var payment = await paymentRepository.GetByIdAsync(spec, ct);

            if (payment is null)
            {
                return Result.Fail(Error.NotFound("Payment.NotFound", $"No payment found for intent '{webhookEvent.PaymentIntentId}'."));
            }

            payment.Status = webhookEvent.EventType == PaymentWebhookEventType.Succeeded
                ? PaymentStatus.Succeeded
                : PaymentStatus.Failed;

            paymentRepository.Update(payment);

            payment.Order.Status = webhookEvent.EventType == PaymentWebhookEventType.Succeeded
                ? OrderStatus.PaymentReceived
                : OrderStatus.PaymentFailed;

            orderRepository.Update(payment.Order);

            if (webhookEvent.EventType == PaymentWebhookEventType.Succeeded)
            {
                var productRepository = unitOfWork.GetRepository<Product, int>();
                var obtainProductsSpec = new ProductWithIdSpecification(payment.Order.Item.Select(i => i.Product.ProductId).ToHashSet());

                var products = await productRepository.GetAllAsync(obtainProductsSpec, ct);

                foreach (var orderItem in payment.Order.Item)
                {
                    var product = products.FirstOrDefault(x => x.Id == orderItem.Product.ProductId);

                    if (product is null)
                    {
                        return Result.Fail(Error.NotFound("Product.NotAvailable", $"Product '{orderItem.Product.ProductName}' is no longer available."));
                    }

                    if (product.QuantityInStock < orderItem.Quantity)
                    {
                        return Result.Fail(Error.Conflict("Stock.Insufficient",
                            $"Only {product.QuantityInStock} unit(s) of '{product.Name}' left in stock but {orderItem.Quantity} were ordered."));
                    }

                    product.QuantityInStock -= orderItem.Quantity;

                    productRepository.Update(product);
                }
            }

            processedEventRepository.Add(new ProcessedWebhookEvent { StripeEventId = webhookEvent.StripeEventId });

            try
            {
                await unitOfWork.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                // The stock quantities were changed by a concurrent transaction after we read them.
                // Nothing was persisted, so it is safe to fail the webhook and let Stripe retry.
                return Result.Fail(Error.Conflict("Stock.ConcurrencyConflict",
                    "Stock changed while processing this payment. Please retry."));
            }

            return Result.OK();
        }
    }
}