# Copilot Instructions  Expense Tracker

## Project Overview
Expense Tracker web app: users log personal expenses by category. Admins manage categories.

## Agents
- **Product Owner**  task decomposition, prompt writing, file updates, progress tracking. Load with `#product-owner`.
- **Backend Developer**  .NET 8 Web API implementation. Load with `#backend-dev`.
- **UI Developer**  React + Vite + TypeScript implementation. Load with `#ui-dev`.
---

## Frontend Tech Stack
| Concern | Technology |
|---------|-----------|
| UI Framework | React 18 (JSX) |
| Routing | React Router DOM 6 |
| Styling | Tailwind CSS 3 + PostCSS + CSS Modules (.module.css) |
| Forms | React Hook Form + Yup (validation) + @hookform/resolvers |
| Data fetching | SWR (caching/revalidation) + Axios (HTTP client) |
| Utilities | lodash (selective), date-fns, classnames, query-string, b64-to-blob |
| Linting/Formatting | ESLint 9 + Prettier + husky + lint-staged (pre-commit hooks) |

## Data Models (single source of truth)

### User
| Field        | Type   | Notes                 |
|--------------|--------|-----------------------|
| Id           | int    | PK                    |
| Email        | string | required, unique      |
| PasswordHash | string | BCrypt hash           |
| Role         | string | `"User"` or `"Admin"` |

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
| Method | Endpoint             | Auth       | Body              | Response     |
|--------|----------------------|------------|-------------------|--------------|
| GET    | /api/categories      | public     |                  | 200 + array  |
| POST   | /api/categories      | Admin only | `{ name, color }` | 201 + object |
| PUT    | /api/categories/{id} | Admin only | `{ name, color }` | 204          |
| DELETE | /api/categories/{id} | Admin only |                  | 204          |

---

## Auth Flow
- JWT stored in `localStorage` under key `token`
- Payload claims: `sub` (userId as string), `email`, `role`
- First registered user  `Role = "Admin"`, all subsequent  `Role = "User"`
- Token expires in 7 days
- Frontend attaches token as `Authorization: Bearer <token>` header on every request
