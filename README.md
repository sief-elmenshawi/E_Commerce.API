# E-Commerce API

RESTful e-commerce backend built with **ASP.NET Core (.NET 10)** and **Onion Architecture**, backed by **EF Core**, **SQL Server**, and **Redis**.

React storefront (separate repo): **[E_Commerce.Web](https://github.com/sief-elmenshawi/E_Commerce.Web)**

---

## Features

- JWT authentication with **refresh-token rotation** (HttpOnly cookie)
- Basket stored in **Redis**, keyed by the authenticated user's id
- Orders + checkout flow with **Stripe** payments
- Products catalog with pagination, filtering, sorting, and Redis caching
- Specification Pattern, FluentValidation, API versioning, rate limiting
- Serilog logging, health checks, Swagger, structured `ProblemDetails` errors

## Tech Stack

| Area | Technology |
| --- | --- |
| Runtime | ASP.NET Core Web API (.NET 10) |
| Data | EF Core (Code First, Migrations) + SQL Server (2 DBs: `ECommerce`, `ECommerceIdentity`) |
| Cache / Basket | Redis |
| Auth | ASP.NET Core Identity + JWT + hashed/rotated refresh tokens |
| Payments | Stripe (`PaymentIntent` + verified webhooks) |
| Logging | Serilog (console + rolling file sinks) |
| Validation | FluentValidation |
| Tests | xUnit, Moq, FluentAssertions |

## Architecture

Onion Architecture across four projects:

| Project | What it contains |
| --- | --- |
| `E_Commerce.API` | Controllers, middleware, global exception handling, `Program.cs` |
| `E_Commerce.Application` | Services, use-cases, DTOs, specifications, validators |
| `E_Commerce.Domain` | Entities, contracts (repositories, unit-of-work) |
| `E_Commerce.Infrastructure` | EF Core contexts, Identity, Redis, Stripe, token services, data seeding |

## Key Behaviours

- **Stock-safe ordering** — inventory is validated and decremented at payment confirmation, guarded by optimistic concurrency (`RowVersion`) to prevent overselling.
- **Basket ownership** — baskets are keyed by the authenticated user's id (JWT `NameIdentifier` claim), never a client-supplied id.
- **Payments** — Stripe webhook signatures are verified and events are processed idempotently (duplicate/retried events are safely ignored).
- **Refresh tokens** — hashed at rest, rotated on every use, delivered via `httpOnly`/`Secure`/`SameSite=Strict` cookies.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or container)
- Redis (local or container: `docker run -p 6379:6379 redis`)
- Stripe account (test mode is fine) for the payment flow

### 1. Configure

Edit `E_Commerce.API/appsettings.json`:

| Key | Notes |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | SQL Server catalog DB (default: `Server=.;Database=ECommerce`) |
| `ConnectionStrings:IdentityConnection` | SQL Server identity DB (default: `Server=.;Database=ECommerceIdentity`) |
| `ConnectionStrings:RedisConnection` | Redis (default: `localhost:6379`) |
| `JWT:SecretKey` | Required — long random string, never commit it |
| `Stripe:SecretKey` / `Stripe:PublishableKey` / `Stripe:WebhookSecret` | Required only for payments |

> **Security:** never commit `JWT:SecretKey` or Stripe keys. Use `dotnet user-secrets` for local dev:

```bash
dotnet user-secrets set "JWT:SecretKey" "your-long-random-secret" --project E_Commerce.API
```

### 2. Run

```bash
dotnet restore
dotnet run --project E_Commerce.API
```

On startup the API **automatically applies pending migrations** to both databases and **seeds** roles, an admin user, and the catalog (brands/types/products/delivery). You can also update migrations manually with `dotnet ef database update`.

- API base URL: **`https://localhost:7175`** (also `http://localhost:5000`)
- Swagger UI: `https://localhost:7175/swagger` (Development only)

## API Overview

All routes are under `api/v1/`.

| Feature | Endpoints |
| --- | --- |
| Authentication | `POST /Authentication/login`, `/register`, `/refreshtoken`, `/logout` · `GET /currentuser`, `/address` · `PUT /address` |
| Products | `GET /Products` (paged/filtered), `GET /Products/{id}`, `/types`, `/brands` |
| Basket | `GET/POST/DELETE /Baskets` (auth required) |
| Orders | `POST /Order`, `GET /Order`, `GET /Order/{id}`, `GET /Order/deliveryMethods` |
| Payments | `POST /Payments`, `POST /Payments/webhook` |

Responses: success returns the payload directly; failures return `application/problem+json` with structured `Error` objects.

## Testing

```bash
dotnet test
```