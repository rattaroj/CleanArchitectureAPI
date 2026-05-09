# Clean Architecture API

A production-ready REST API template built with **ASP.NET Core 8** following **Clean Architecture** principles and **CQRS** pattern.

## Tech Stack

| Category | Library |
|---|---|
| Framework | ASP.NET Core 8 |
| CQRS / Mediator | MediatR 14 |
| Validation | FluentValidation 12 |
| API Endpoints | Ardalis.ApiEndpoints 4 |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL (Npgsql 8) |
| API Docs | Swagger / Swashbuckle with Annotations |
| Testing | xUnit + coverlet |

---

## Project Structure

```
src/
├── CleanArchitecture.Domain/          # Entities, Value Objects, Domain Events — no external deps
├── CleanArchitecture.Application/     # CQRS Commands/Queries, Validators, Repository interfaces
├── CleanArchitecture.Infrastructure/  # EF Core, PostgreSQL, Repository implementations
└── CleanArchitecture.API/             # Endpoints, Middleware, Swagger, Contracts

tests/
├── CleanArchitecture.UnitTests/
└── CleanArchitecture.IntegrationTests/
```

Dependency direction: `API → Application → Domain` and `Infrastructure → Application + Domain`

---

## Architecture Overview

### Clean Architecture Layers

**Domain** — Core business logic. Contains `User` entity, `Email` value object, `UserCreatedDomainEvent`, and `DomainException`. Zero external dependencies.

**Application** — Use-case layer. Each feature follows CQRS: a `Command`/`Query` record + `Handler` + `FluentValidation` validator. A MediatR `ValidationBehavior` pipeline runs all validators automatically. Repository and Unit of Work are defined here as interfaces.

**Infrastructure** — Persistence. `ApplicationDbContext` (EF Core), `UserRepository`, `UnitOfWork`, and EF migrations. No business logic lives here.

**API** — HTTP surface. Uses `Ardalis.ApiEndpoints` — each endpoint is its own class, no traditional controllers. All responses are wrapped in `ApiResponse<T>`. `ExceptionHandlingMiddleware` handles unhandled exceptions globally.

### Key Patterns

- **Result\<T\>** — handlers return `Result<T>` instead of throwing exceptions; mapped to HTTP status codes in the API layer
- **Value Objects** — `Email` is validated at construction in the Domain layer
- **Domain Events** — `UserCreatedDomainEvent` raised inside `User.Create()`
- **Repository + Unit of Work** — abstractions in Application, implementations in Infrastructure

---

## API Endpoints

### Users

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/users` | Create a new user |
| `GET` | `/api/users` | Get paginated list of users (with filtering & sorting) |
| `GET` | `/api/users/{id}` | Get user by ID |
| `DELETE` | `/api/users/{id}` | Delete a user |

### Query Parameters for `GET /api/users`

| Parameter | Type | Description |
|---|---|---|
| `pageNumber` | int | Page number (default: 1) |
| `pageSize` | int | Items per page (default: 10) |
| `sortBy` | string | `name` \| `email` \| `isactive` (default: `name`) |
| `direction` | string | `asc` \| `desc` (default: `asc`) |
| `nameContains` | string | Filter by name (partial match) |
| `emailEquals` | string | Filter by exact email |
| `isActive` | bool | Filter by active status |

### Response Envelope

```json
{
  "success": true,
  "message": "Success",
  "data": { },
  "error": null,
  "pagination": {
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 42,
    "totalPages": 5
  }
}
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL running locally

### Configuration

Update the connection string in `src/CleanArchitecture.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=user_db;Username=postgres;Password=yourpassword"
  }
}
```

### Run

```bash
# Apply database migrations
dotnet ef database update --project src/CleanArchitecture.Infrastructure --startup-project src/CleanArchitecture.API

# Run the API
dotnet run --project src/CleanArchitecture.API
```

Swagger UI is available at `https://localhost:{port}/swagger` in Development mode.

### Test

```bash
dotnet test
```

---

## Adding a New Feature

Follow this pattern when adding a new domain feature:

1. **Domain** — add entity/value object if needed
2. **Application** — create `Features/{Domain}/Commands|Queries/{FeatureName}/`
   - `{Feature}Command.cs` or `{Feature}Query.cs` (record implementing `IRequest<Result<T>>`)
   - `{Feature}CommandHandler.cs` implementing `IRequestHandler`
   - `{Feature}CommandValidator.cs` extending `AbstractValidator`
3. **Infrastructure** — add repository method if needed
4. **API** — add `{Feature}Endpoint.cs` in `Endpoints/{Domain}/` and matching contract in `Contracts/`
