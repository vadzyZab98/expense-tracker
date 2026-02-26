# Copilot Instructions  Expense Tracker

## Project Overview
Expense Tracker web app: users log personal expenses by category. Admins manage categories. SuperAdmin manages user roles.
Backend uses **Onion Architecture** with **CQRS** (MediatR), **FluentValidation**, and **Repository pattern**.

## Agents
- **Backend Developer**  .NET 8 Web API implementation. Load with `#backend-dev`.
- **UI Developer**  React + Vite + TypeScript implementation. Load with `#ui-dev`.

---

## Mandatory Documentation Checklist

**Every feature, refactor, or structural change MUST finish with these steps (no exceptions):**

1. **Update the relevant prompt file** — if the change adds/removes/renames entities, endpoints, commands, queries, validators, folders, packages, or conventions, reflect it in the matching `.github/prompts/*.prompt.md` file so the agent instructions stay accurate.
2. **Add a step to `README.md`** — append a new `### Step N` entry in the Prompt History section using the standard format (Agent, Prompt, Result, Accepted/Changed).
3. **Update `copilot-instructions.md`** — if the change affects the solution structure, data models, API contract, auth flow, or roles sections, update them here.

> If any of the above steps are missing, the task is **not complete**.

---

## Ports
| Service  | URL                   |
|----------|-----------------------|
| Backend  | http://localhost:5001 |
| Frontend | http://localhost:5173 |

---

## Solution Structure

```
server/
  ExpenseTracker.sln
  shared/                            ← .editorconfig
  src/
    ExpenseTracker.Core/             ← Entities, interfaces (zero dependencies)
    ExpenseTracker.Logic/            ← CQRS commands/queries (MediatR), DTOs, co-located validators, Result pattern
    ExpenseTracker.Persistence/      ← EF Core DbContext, repositories
    ExpenseTracker.Api/              ← Thin controllers (MediatR dispatch), auth services (JWT, BCrypt), composition root
  tests/
    ExpenseTracker.UnitTests/        ← xUnit + Moq + FluentAssertions
```

Dependency direction: **Api → Persistence → Logic → Core**

Error handling: **Result&lt;T&gt; pattern** (no domain exceptions). Handlers return `Result` / `Result<T>` with `DomainError` (NotFound, Conflict, Unauthorized). Controllers use `ApiControllerBase.MapError()` to convert errors to ProblemDetails.

---

## Data Models (single source of truth)

### User
| Field        | Type   | Notes                 |
|--------------|--------|-----------------------|
| Id           | int    | PK                    |
| Email        | string | required, unique      |
| PasswordHash | string | BCrypt hash           |
| Role         | string | `"User"`, `"Admin"`, or `"SuperAdmin"` |

### Category
| Field | Type   | Notes                       |
|-------|--------|-----------------------------|
| Id    | int    | PK                          |
| Name  | string | required                    |
| Color | string | hex color, e.g. `"#FF6B6B"` |

### Expense
| Field       | Type     | Notes            |
|-------------|----------|------------------|
| Id          | int      | PK               |
| UserId      | int      | FK  User.Id     |
| CategoryId  | int      | FK  Category.Id |
| Amount      | decimal  | required         |
| Description | string   | required         |
| Date        | DateTime | required         |

---

## API Contract

### Auth  no token required
| Method | Endpoint           | Body                  | Response          |
|--------|--------------------|-----------------------|-------------------|
| POST   | /api/auth/register | `{ email, password }` | 201 + `{ token }` |
| POST   | /api/auth/login    | `{ email, password }` | 200 + `{ token }` |

### Expenses  Bearer token required
| Method | Endpoint           | Body                                        | Response     |
|--------|--------------------|---------------------------------------------|--------------|
| GET    | /api/expenses      |                                            | 200 + array  |
| POST   | /api/expenses      | `{ amount, description, date, categoryId }` | 201 + object |
| PUT    | /api/expenses/{id} | `{ amount, description, date, categoryId }` | 204          |
| DELETE | /api/expenses/{id} |                                            | 204          |

### Categories
| Method | Endpoint             | Auth                  | Body              | Response     |
|--------|----------------------|-----------------------|-------------------|--------------|
| GET    | /api/categories      | public                |                  | 200 + array  |
| POST   | /api/categories      | Admin or SuperAdmin   | `{ name, color }` | 201 + object |
| PUT    | /api/categories/{id} | Admin or SuperAdmin   | `{ name, color }` | 204          |
| DELETE | /api/categories/{id} | Admin or SuperAdmin   |                  | 204          |

### Users  SuperAdmin only
| Method | Endpoint              | Body           | Response     |
|--------|-----------------------|----------------|--------------|
| GET    | /api/users            |               | 200 + array  |
| PUT    | /api/users/{id}/role  | `{ role }`     | 204          |

---

## Auth Flow
- JWT stored in `localStorage` under key `token`
- Payload claims: `sub` (userId as string), `email`, `role`
- A **SuperAdmin** user (`admin@mail.ru` / `12345678`) is seeded via migration
- All registered users get `Role = "User"` by default
- SuperAdmin can promote users to `Admin` or revoke back to `User`
- Token expires in 7 days
- Frontend attaches token as `Authorization: Bearer <token>` header on every request

---

## Roles
| Role       | Permissions                                      |
|------------|--------------------------------------------------|
| SuperAdmin | Manage user roles, manage categories, all access |
| Admin      | Manage categories, own expenses                  |
| User       | Own expenses only                                |
