using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Services
{
    internal class OrderServices : IOrderService
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IBasketRepository basketRepository;

        public OrderServices(IMapper mapper,IUnitOfWork unitOfWork, IBasketRepository basketRepository )
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.basketRepository = basketRepository;
        }
        public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken ct = default)
        {
            // 1-Validate Basket Found & Item
            var basket = await basketRepository.GetBasketAsync(orderDto.BasketId,ct);

            if (basket is null)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Basket Not Found", $"Basket With {orderDto.BasketId} Is Not Found"));
            
            if (basket.Items.Count == 0 )
                return Result<OrderToReturnDto>.Fail(Error.Validation("Basket Empty", $"Basket Id Empty "));

            // 2-Get Item From Basket Validate as Product

            var productRepo = unitOfWork.GetRepository<Product, int>();

            var productId = basket.Items.Select(x => x.Id).ToHashSet();

            var products= await productRepo.GetAllAsync(new ProductWithIdSpecification(productId),ct);

            var orderItem = new List<OrderItem>(basket.Items.Count);

            foreach(var item in basket.Items)
            {
                var product = products.FirstOrDefault(x => x.Id == item.Id);

                if(product is null)
                    return Result<OrderToReturnDto>.Fail(Error.NotFound("Product Not Found", $"Product With {product?.Name} Is Not Found"));

                orderItem.Add(new OrderItem()
                {
                    Price = product.Price,
                    Quantity =item.Quantity,
                    Product = new ProductItemOrdered()
                    {
                        ProductId = product.Id,
                        PictureUrl = product.PictureUrl,
                        ProductName = product.Name,
                    }
                });



            }

            // 3- Store Order Address
            var orderAddress = mapper.Map<OrderAddress>(orderDto.ShippingAddress);

            // 4- store DeliveryMethod
            var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(orderDto.DeliveryMethodId, ct);
            if(deliveryMethod is null)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Delivery Not Found", $"Delivery With {orderDto.DeliveryMethodId} Is Not Found"));

            // 5-Calculation 
            var subTotal = orderItem.Sum(x => x.Price * x.Quantity);

            // 6- Generate ORder 
            var order = new Order()
            {
                BuyerEmail = email,
                Item = orderItem,
                ShippingAddress = orderAddress,
                DeliveryMethod = deliveryMethod, 
                DeliveryMethodId = deliveryMethod.Id,
                SubTotal = subTotal,

            };

            unitOfWork.GetRepository<Order,Guid>().Add(order); 
            var result = await unitOfWork.SaveChangesAsync();

            if (result <= 0)
                return Result<OrderToReturnDto>.Fail(Error.Failure("Order Failure", $"Order Can Not Created"));

            await basketRepository.DeleteBasketAsync(orderDto.BasketId,ct);

            return Result<OrderToReturnDto>.Ok(mapper.Map<OrderToReturnDto>(order));


        }

        public async Task<Result<IReadOnlyList<DeliveryMethodDto>>> GetAllDeliveryMethodsAsync(CancellationToken ct = default)
        {
            var deliveryMethods = await unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync(ct);
            return Result<IReadOnlyList<DeliveryMethodDto>>.Ok(mapper.Map<IReadOnlyList<DeliveryMethodDto>>(deliveryMethods));
        }

        public async Task<Result<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersByEmailAsync(string email, CancellationToken ct = default)
        {
            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync(new OrderSpecification(email));
            return Result<IReadOnlyList<OrderToReturnDto>>.Ok(mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders));

        }

        public async Task<Result<OrderToReturnDto>> GetOrderByIdAndEmailAsync(Guid Id, string email, CancellationToken ct = default)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(new OrderSpecification(Id,email));
            if (order is null)
                return Error.NotFound("OrderNotFound", $"Order Not Found {Id}");

            return Result<OrderToReturnDto>.Ok(mapper.Map<OrderToReturnDto>(order));
        }
    }
}
