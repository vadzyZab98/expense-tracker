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

### IncomeCategory
| Field | Type   | Notes                       |
|-------|--------|-----------------------------|
| Id    | int    | PK                          |
| Name  | string | required, max 100           |
| Color | string | hex color, max 7            |

### Income
| Field            | Type     | Notes                     |
|------------------|----------|---------------------------|
| Id               | int      | PK                        |
| UserId           | int      | FK → User.Id              |
| IncomeCategoryId | int      | FK → IncomeCategory.Id    |
| Amount           | decimal  | required, > 0             |
| Date             | DateTime | required                  |

### MonthlyBudget
| Field      | Type    | Notes                                          |
|------------|---------|-------------------------------------------------|
| Id         | int     | PK                                              |
| UserId     | int     | FK → User.Id                                    |
| CategoryId | int     | FK → Category.Id (expense category)             |
| Year       | int     | required, 2000–2100                             |
| Month      | int     | required, 1–12                                  |
| Amount     | decimal | required, > 0                                   |

Unique constraint: `(UserId, CategoryId, Year, Month)` on MonthlyBudget.

---

## Financial Constraints (Income Enforcement)

Every month's budgets and expenses are bounded by the user's total income for that month.

| Operation          | Rule                                                                                 | Error   |
|--------------------|--------------------------------------------------------------------------------------|---------|
| Create Expense     | `totalExpenses(month) + amount ≤ totalIncome(month)`; rejected if income is zero      | 409     |
| Update Expense     | Same check on target month when amount increases or month changes                     | 409     |
| Create Budget      | `totalBudgets(month) + amount ≤ totalIncome(month)`; rejected if income is zero       | 409     |
| Update Budget      | Same check on target month when amount increases or month changes                     | 409     |
| Delete Income      | Rejected if `remainingIncome < totalBudgets(month)` or `< totalExpenses(month)`       | 409     |
| Update Income      | If amount decreases or month changes: same guard on the old month                     | 409     |

Equality is allowed (`total == income` is valid). Checks are per-month only, no cross-month aggregation.

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

Expense creation/update enforces income constraint: `totalExpenses(month) + amount ≤ totalIncome(month)`. Returns 409 if violated or if income is zero.

### Categories
| Method | Endpoint             | Auth                  | Body              | Response     |
|--------|----------------------|-----------------------|-------------------|--------------|
| GET    | /api/categories      | public                |                  | 200 + array  |
| GET    | /api/categories/{id} | public                |                  | 200 + object |
| POST   | /api/categories      | Admin or SuperAdmin   | `{ name, color }` | 201 + object |
| PUT    | /api/categories/{id} | Admin or SuperAdmin   | `{ name, color }` | 204          |
| DELETE | /api/categories/{id} | Admin or SuperAdmin   |                  | 204          |

### Income Categories
| Method | Endpoint                    | Auth                  | Body              | Response     |
|--------|-----------------------------|-----------------------|-------------------|--------------|
| GET    | /api/income-categories      | public                |                  | 200 + array  |
| GET    | /api/income-categories/{id} | public                |                  | 200 + object |
| POST   | /api/income-categories      | Admin or SuperAdmin   | `{ name, color }` | 201 + object |
| PUT    | /api/income-categories/{id} | Admin or SuperAdmin   | `{ name, color }` | 204          |
| DELETE | /api/income-categories/{id} | Admin or SuperAdmin   |                  | 204          |

### Incomes  Bearer token required
| Method | Endpoint          | Body                                     | Response     |
|--------|-------------------|------------------------------------------|--------------|
| GET    | /api/incomes      |                                         | 200 + array  |
| GET    | /api/incomes/{id} |                                         | 200 + object |
| POST   | /api/incomes      | `{ amount, date, incomeCategoryId }`     | 201 + object |
| PUT    | /api/incomes/{id} | `{ amount, date, incomeCategoryId }`     | 204 or 409   |
| DELETE | /api/incomes/{id} |                                         | 204 or 409   |

Income update/delete enforces guards: reject if reducing/removing income would cause budgets or expenses to exceed remaining income.

### Monthly Budgets  Bearer token required
| Method | Endpoint                     | Body                                       | Response     |
|--------|------------------------------|--------------------------------------------|--------------|
| GET    | /api/budgets                 |                                           | 200 + array  |
| GET    | /api/budgets?year=Y&month=M |                                           | 200 + array  |
| POST   | /api/budgets                 | `{ categoryId, year, month, amount }`      | 201 + object |
| PUT    | /api/budgets/{id}            | `{ categoryId, year, month, amount }`      | 204 or 409   |
| DELETE | /api/budgets/{id}            |                                           | 204          |

Budget creation/update enforces: `totalBudgets(month) + amount ≤ totalIncome(month)`. Returns 409 if violated or if income is zero.

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
