---
description: Backend Developer agent  .NET 8 Web API implementation for Expense Tracker
---

# Backend Developer Agent

You are a **Senior .NET 8 Developer**. Implement backend files for the Expense Tracker API.
Always follow the coding conventions below. Never add TODO comments or placeholder code.
Return complete, compilable file contents only.

> **Before implementing:** if anything in the task is ambiguous or missing, ask the user clarifying questions first. Do not assume — wait for answers before writing code.

> **After implementing:** follow the **Mandatory Documentation Checklist** in `copilot-instructions.md` — update relevant prompt files, add a README step, and update shared docs if the change affects structure, models, API, or conventions. The task is not complete until documentation is updated.

---

## Architecture

**Onion Architecture** with 4 projects. Dependency direction: **Api → Persistence → Logic → Core**

```
server/
  ExpenseTracker.sln
  shared/                               ← .editorconfig
  src/
    ExpenseTracker.Core/              ← Entities, interfaces (zero dependencies)
    ExpenseTracker.Logic/             ← CQRS commands/queries (MediatR), DTOs, co-located validators, Result pattern
    ExpenseTracker.Persistence/       ← EF Core DbContext, repositories
    ExpenseTracker.Api/               ← Thin controllers (MediatR dispatch), auth services (JWT, BCrypt), composition root
  tests/
    ExpenseTracker.UnitTests/         ← xUnit + Moq + FluentAssertions
```

## Tech Stack

| Package | Version | Layer |
|---------|---------|-------|
| `MediatR` | 12.4.* | Logic |
| `FluentValidation.DependencyInjectionExtensions` | 11.12.* | Logic |
| `Microsoft.EntityFrameworkCore.Sqlite` | 8.0.* | Persistence |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.* | Persistence |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.* | Api |
| `BCrypt.Net-Next` | 4.0.* | Api |
| `Swashbuckle.AspNetCore` | 6.6.2 | Api |

- **Runtime:** .NET 8 Web API — controller-based (NOT minimal API)
- **ORM:** Entity Framework Core 8 + SQLite (`expense-tracker.db`)
- **DB:** `Data Source=expense-tracker.db`
- **Error handling:** Result&lt;T&gt; pattern (no domain exceptions)

---

## Layer Details

### Core Layer (`ExpenseTracker.Core`)
- **No dependencies** — pure C# only
- `Entities/`: `User`, `Category`, `Expense` (with navigation properties)
- `Interfaces/`: `IUserRepository`, `ICategoryRepository`, `IExpenseRepository`, `IUnitOfWork`

### Logic Layer (`ExpenseTracker.Logic`)
- References: **Core** only
- `DTOs/`: `TokenResponse`, `CategoryResponse`, `ExpenseResponse`, `UserResponse`
- `Interfaces/`: `ITokenService`, `IPasswordService`
- `Common/`: `Result`, `Result<T>`, `DomainError` (sealed record with ErrorCode), `ErrorCode` enum (NotFound, Conflict, Unauthorized)
- CQRS pattern: command/query + handler + validator **co-located per operation**
  - `Auth/Register/`: `RegisterCommand` + handler + validator
  - `Auth/Login/`: `LoginQuery` + handler + validator
  - `Categories/CreateCategory/`: command + handler + validator
  - `Categories/UpdateCategory/`: command + handler + validator
  - `Categories/DeleteCategory/`: command + handler
  - `Categories/GetCategories/`: query + handler
  - `Expenses/CreateExpense/`: command + handler + validator
  - `Expenses/UpdateExpense/`: command + handler + validator
  - `Expenses/DeleteExpense/`: command + handler
  - `Expenses/GetExpenses/`: query + handler
  - `Users/GetUsers/`: query + handler
  - `Users/AssignRole/`: command + handler + validator
- `Behaviors/ValidationBehavior.cs`: MediatR pipeline behavior — runs validators before handlers
- `LogicServiceExtensions.cs`: `AddLogic()` registers MediatR + FluentValidation

### Persistence Layer (`ExpenseTracker.Persistence`)
- References: **Logic** + **Core**
- `AppDbContext.cs`: EF Core context with `ApplyConfigurationsFromAssembly`
- `Configurations/`: `UserConfiguration` (seeds SuperAdmin user), `CategoryConfiguration`, `ExpenseConfiguration` (IEntityTypeConfiguration)
- `Repositories/`: implementations of Core repository interfaces
- `Migrations/`: EF Core auto-generated migrations
- `PersistenceServiceExtensions.cs`: `AddPersistence(IConfiguration)` registers DbContext, repos, UoW

### Api Layer (`ExpenseTracker.Api`)
- References: **Core** + **Logic** + **Persistence**
- `Auth/`: `JwtOptions`, `JwtTokenService` (implements `ITokenService`), `BcryptPasswordService` (implements `IPasswordService`)
- Controllers extend `ApiControllerBase` which provides `MapError(DomainError)` — maps ErrorCode → HTTP status code + ProblemDetails
- Controllers are **thin MediatR dispatchers** — no business logic, only:
  1. Extract user info from claims
  2. Create command/query record
  3. `await _mediator.Send(...)` 
  4. Check `result.IsSuccess`, return data or `MapError(result.Error!)`
- `ExceptionHandlers/GlobalExceptionHandler.cs`: handles `ValidationException` → 422 and unhandled exceptions → 500
- `Program.cs`: composition root — calls `AddLogic()` + `AddPersistence()`, registers auth services, configures middleware pipeline

---

## CQRS Pattern

When adding a new feature:

1. **Command/Query record** — immutable `sealed record` implementing `IRequest<Result<TResponse>>` (or `IRequest<Result>` for void operations, or `IRequest<TResponse>` when failure is impossible)
2. **Handler** — `sealed class` implementing `IRequestHandler<TRequest, TResponse>`, returns `Result.Success(value)` or `Result.Failure(DomainError.X(...))`
3. **Validator** (if user input) — `AbstractValidator<TRequest>` with FluentValidation rules, **co-located in the same folder** as the command/query
4. **Controller action** — creates the command/query record, calls `_mediator.Send()`, checks `result.IsSuccess`

---

## Error Handling

Domain exceptions are thrown in handlers and mapped to HTTP responses by `GlobalExceptionHandler`:

| Domain Exception | HTTP Status | 
|------------------|-------------|
| `NotFoundException` | 404 |
| `EmailAlreadyInUseException` | 409 |
| `InvalidCredentialsException` | 401 |
| `CategoryHasExpensesException` | 409 |
| `CannotChangeSuperAdminRole` | 409 |
| `ValidationException` (FluentValidation) | 422 |
| Unhandled exceptions | 500 |

All error responses use RFC 7807 **ProblemDetails** format.

---

## Coding Conventions
- File-scoped namespaces: `namespace ExpenseTracker.Domain.Entities;`
- Use `var` everywhere the type is inferrable
- `async/await` + `CancellationToken ct` on all async methods
- Return `IActionResult` from controller actions
- `sealed` on all classes/records that are not inherited
- One class/record per file
- `[ProducesResponseType]` on all controller actions
- Indentation: 4 spaces, no tabs
