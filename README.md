# E-Commerce API

RESTful E-Commerce backend built with **ASP.NET Core** and **Onion Architecture**. The frontend storefront lives in a separate repository: **[E_Commerce.Web](https://github.com/sief-elmenshawi/E_Commerce.Web)**.

## Tech Stack

- **ASP.NET Core Web API** (.NET 10)
- **EF Core** (Code First, Migrations) + **SQL Server**
- **Redis** — response caching + distributed cache
- **ASP.NET Core Identity** + **JWT** (access tokens) + hashed, rotated **Refresh Tokens** (httpOnly, Secure, SameSite=Strict cookies)
- **Stripe** — PaymentElement flow, webhook signature verification, idempotent event handling
- **Serilog** — structured logging (console + rolling file sinks)
- **FluentValidation**, **Global Exception Handling**, **ProblemDetails**, **API Versioning**, **Rate Limiting**, **Health Checks**
- **xUnit**, **Moq**, **FluentAssertions** — unit tests

## Architecture

Onion Architecture across four projects:

- `E_Commerce.Domain` — entities, no external dependencies
- `E_Commerce.Application` — services, DTOs, specifications, contracts (Repository / Unit of Work / Specification patterns)
- `E_Commerce.Infrastructure` — EF Core, Identity, Redis, Stripe, token services
- `E_Commerce.API` — controllers, middleware, composition root

## Key Behaviours

- **Stock-safe ordering** — inventory is validated and decremented at payment confirmation, guarded by optimistic concurrency (`RowVersion`) to prevent overselling.
- **Basket ownership** — baskets are keyed by the authenticated user's id (from the JWT `NameIdentifier` claim), not a client-supplied id.
- **Payments** — Stripe webhook signatures are verified and events are processed idempotently (duplicate/retried events are safely ignored).
- **Refresh tokens** are hashed at rest and rotated on every use; delivered via httpOnly/Secure/SameSite=Strict cookies.

## Prerequisites

- .NET 10 SDK
- SQL Server (local or container)
- Redis (local or container)
- A Stripe account (test mode is fine) for the payment flow

## Configuration

Secrets are **not** committed. Set the following via `dotnet user-secrets` (recommended for local dev) or environment variables — see `appsettings.json` for the full shape:
