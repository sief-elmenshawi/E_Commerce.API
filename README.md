# E-Commerce API

Robust .NET 10 Web API for an e-commerce platform. Clean Architecture with CQRS-free layering:

- **E_Commerce.API** — ASP.NET Core Web API (controllers, middleware, attributes, `Program.cs`)
- **E_Commerce.Application** — use-cases, services, DTOs, specifications, FluentValidation validators
- **E_Commerce.Domain** — entities, contracts (repositories, unit-of-work), value objects
- **E_Commerce.Infrastructure** — EF Core `StoreDbContext`, Identity `AppIdentityDbContext`, Redis caching, Stripe payment gateway, data seeding
- **E_Commerce.Tests** — xUnit unit tests

The React frontend lives in a separate repository: `D:\Assignment\E_Commerce.Web` — see its repo at **[github.com/sief-elmenshawi/E_Commerce.Web](https://github.com/sief-elmenshawi/E_Commerce.Web)**. That frontend is the storefront for **this** API.

## Tech Stack

- **.NET 10** / ASP.NET Core Web API
- **Entity Framework Core 10** (two databases: `ECommerce` + `ECommerceIdentity`)
- **ASP.NET Core Identity** (JWT bearer + refresh-token rotation with HttpOnly cookie)
- **Stripe** (`PaymentIntent` flow + webhooks)
- **Redis** (response caching via `[RedisCache]` filter)
- **FluentValidation** + automatic model validation
- **Serilog** (console + rolling file `Logs/log-.txt`)
- **Swagger / OpenAPI** (available in Development only)
- **API Versioning** (`api/v1/...`)
- **Health checks** + **Rate limiting**

## Prerequisites

- .NET SDK 10
- SQL Server (local instance, Windows auth by default)
- Redis server (or Docker)
- Stripe account (optional if you only want the catalog/auth to work)

## Configuration

All settings live in `E_Commerce.API/appsettings.json`:

| Section | Key | Notes |
| --- | --- | --- |
| `ConnectionStrings:DefaultConnection` | SQL Server catalog DB | |
| `ConnectionStrings:IdentityConnection` | SQL Server Identity DB | |
| `ConnectionStrings:RedisConnection` | Redis connection string | |
| `UrlSettings:BaseUrl` | Public base URL | |
| `JWT:SecretKey` | JWT signing secret | **change in production** |
| `JWT:Issuer` / `JWT:Audience` | Token claims | |
| `Stripe:SecretKey` | Stripe secret key | |
| `Stripe:PublishableKey` | Stripe publishable key | |
| `Stripe:WebhookSecret` | Stripe webhook signing secret | |

> **Security note:** never commit production secrets. Use user-secrets / env vars / secret manager for `JWT:SecretKey`, `Stripe:SecretKey` and `Stripe:WebhookSecret`.

## How to Run

```bash
dotnet restore
dotnet build
dotnet run --project E_Commerce.API
```

The API listens on `https://localhost:7175` by default.

On startup the app will:

1. Apply any pending EF migrations and migrate both databases.
2. Seed the Identity roles (`Customer`, `Admin`, `SuperAdmin`) and an admin user.
3. Seed the catalog from `E_Commerce.API/DataSeed/*.json` (`brands`, `types`, `products`, `delivery`) when the corresponding tables are empty. The JSON files are copied to the output directory automatically (`CopyToOutputDirectory="PreserveNewest"`).

Open Swagger at `https://localhost:7175/swagger` while running in the Development environment.

## Testing (tests)

```bash
dotnet test
```

Run the included xUnit tests (e.g. refresh-token service tests).

## API Overview

All routes are versioned under `api/v1/`.

### Authentication — `api/v1/Authentication`

| Method | Route | Auth | Description |
| --- | --- | --- | --- |
| POST | `login` | No | Sign in, sets `refreshToken` HttpOnly cookie (rate-limited) |
| POST | `register` | No | Register (rate-limited) |
| POST | `refreshtoken` | Cookie | Rotate refresh token, issue new access token |
| POST | `logout` | Cookie | Revoke refresh token, clear cookie |
| GET | `emailexists` | No | Check email availability |
| GET | `currentuser` | Yes | Current user profile |
| GET | `address` | Yes | Get saved shipping address |
| PUT | `address` | Yes | Create/update shipping address |

### Products — `api/v1/Products`

- `GET /` — paged, filtered, sorted list with Redis caching (90s) — query: `brandId`, `typeId`, `searchValue`, `sort` (`NameAcc`, `NameDesc`, `PriceAcc`, `PriceDesc`), `pageIndex`, `pageSize`.
- `GET /{id}` — product by id.
- `GET /types`, `GET /brands` — filter options.

### Orders — `api/v1/Order`

| Method | Route | Auth | Description |
| --- | --- | --- | --- |
| GET | `deliveryMethods` | No | Delivery methods |
| POST | `` | Yes | Create order from a basket (email taken from JWT) |
| GET | `` | Yes | Current user's orders |
| GET | `/{id}` | Yes | Order detail (only for the owning user) |

### Payments — `api/v1/Payments`

| Method | Route | Auth | Description |
| --- | --- | --- | --- |
| POST | `` | Yes | Create/update a Stripe PaymentIntent for **your own** order (ownership enforced via JWT email) |
| POST | `webhook` | No | Stripe webhook (verified by signature) |

Result envelope: success returns the payload directly; failures return `application/problem+json` whose `errors` extension holds strongly-typed `Error` objects.

## Error Handling

- `Error` object: `Code`, `Description`, `ErrorType` (`NotFound`, `Validation`, `Conflict`, `Unauthorized`, `Forbidden`, `Failure`, `Internal`).
- `ApiBaseController.ToProblem` maps error types to HTTP status codes.
- `GlobalExceptionHandler` catches unhandled exceptions and returns a safe `ProblemDetails` (exception message only in Development).

## Security

- **Ownership checks:** `OrderController` and `PaymentsController` derive the email from the JWT and scope all queries to that email.
- **No account enumeration on login:** unknown email and wrong password both return `401 Invalid email or password`.
- **Refresh tokens:** stored as SHA-256 hash, rotated on every refresh, revoked on logout, 7-day lifetime, delivered in an `HttpOnly` + `Secure` + `SameSite=Strict` cookie scoped to `/api`.
- **Rate limiting:** strict policies on `login`/`register` (5/min per IP) plus a global limiter.
- **Redis caching:** `[RedisCache]` degrades gracefully and never breaks requests when Redis is unavailable.