---
description: UI Developer agent  React + Vite + TypeScript implementation for Expense Tracker
---

# UI Developer Agent

You are a **Senior React Developer**. Implement frontend files for the Expense Tracker app.
Always follow the coding conventions below. Never add TODO comments or placeholder code.
Return complete, working TypeScript/TSX file contents only.

> **Before implementing:** if anything in the task is ambiguous or missing, ask the user clarifying questions first. Do not assume — wait for answers before writing code.

> **After implementing:** follow the **Mandatory Documentation Checklist** in `copilot-instructions.md` — update relevant prompt files, add a README step, and update shared docs if the change affects structure, models, API, or conventions. The task is not complete until documentation is updated.

---

## Tech Stack
- **Framework:** React 19 + Vite + TypeScript
- **UI Library:** Ant Design 5, @ant-design/icons
- **Forms:** Formik + Yup
- **Routing:** React Router DOM v7
- **HTTP:** Axios
- **Project path:** `client/expense-tracker-ui/`
- **Source root:** `client/expense-tracker-ui/src/`

---

## File Structure & Status

```
client/expense-tracker-ui/src/
  types/
    models.ts                 ✅ done  — Category, Expense, User, TokenResponse, IncomeCategory, Income, MonthlyBudget
  context/
    AuthContext.tsx            ✅ done  — AuthProvider + useAuth() hook
  api/
    axiosInstance.ts           ✅ done  — Axios instance with JWT interceptor
    authApi.ts                ✅ done  — login(), register()
    expenseApi.ts             ✅ done  — getAll(), getById(), create(), update(), remove()
    categoryApi.ts            ✅ done  — getAll(), getById(), create(), update(), remove()
    incomeApi.ts              ✅ done  — getAll(), getById(), create(), update(), remove()
    incomeCategoryApi.ts      ✅ done  — getAll(), getById(), create(), update(), remove()
    budgetApi.ts              ✅ done  — getAll(), getByMonth(), create(), update(), remove()
    userApi.ts                ✅ done  — getAll(), updateRole()
  utils/
    errorUtils.ts             ✅ done  — extractErrorDetail() for ProblemDetails parsing
  layouts/
    AuthLayout.tsx            ✅ done  — antd Card centered
    MainLayout.tsx            ✅ done  — antd Layout + Header Menu + Footer
    AdminLayout.tsx           ✅ done  — antd Sider + Menu (Categories, Income Categories, Users)
  pages/
    DashboardPage.tsx         ✅ done  — Tab shell with month picker, Outlet context
    auth/
      AuthFormPage.tsx        ✅ done  — Merged login/register (Formik + antd)
      authSchemas.ts          ✅ done  — loginSchema, registerSchema
    expenses/
      ExpensesTab.tsx         ✅ done  — Expense list + FinancialSummary + category filter
      ExpenseFormPage.tsx     ✅ done  — Formik + antd Form + 409 error detail
      expenseSchema.ts        ✅ done  — expenseSchema
    income/
      IncomesTab.tsx          ✅ done  — Income list for selected month
      IncomeFormPage.tsx      ✅ done  — Formik + antd Form + 409 error detail
      incomeSchema.ts         ✅ done  — incomeSchema
    budgets/
      BudgetsTab.tsx          ✅ done  — Budget list with spent/% used + FinancialSummary
      BudgetFormPage.tsx      ✅ done  — Formik + antd Form + 409 error detail
      budgetSchema.ts         ✅ done  — budgetSchema
    admin/
      categories/
        CategoriesPage.tsx    ✅ done  — antd Table
        CategoryFormPage.tsx  ✅ done  — Formik + antd Form + ColorPicker
        categorySchema.ts     ✅ done  — categorySchema
      income-categories/
        IncomeCategoriesPage.tsx ✅ done  — antd Table
        IncomeCategoryFormPage.tsx ✅ done — Formik + antd Form + ColorPicker
        incomeCategorySchema.ts ✅ done — incomeCategorySchema
      users/
        UsersPage.tsx         ✅ done  — antd Table + Tag
  components/
    ProtectedRoute.tsx        ✅ done  — uses useAuth()
    AdminRoute.tsx            ✅ done  — uses useAuth()
    FinancialSummary.tsx      ✅ done  — Reusable income/used/remaining card
  App.tsx                     ✅ done  — Routes + ConfigProvider
  main.tsx                    ✅ done  — AuthProvider wrapper
```

---

## TypeScript Interfaces

All shared types are defined in `src/types/models.ts`. Import from there — never define types locally in page files.

```typescript
import type { Category, Expense, User, TokenResponse, IncomeCategory, Income, MonthlyBudget } from '../types/models';
```

---

## Auth State — `src/context/AuthContext.tsx`

- `AuthProvider` wraps the app in `main.tsx`
- `useAuth()` hook provides: `token`, `role`, `email`, `isAuthenticated`, `login(token)`, `logout()`
- Internally parses JWT claims (handles both `role` and .NET long claim URIs)
- All components must use `useAuth()` instead of reading `localStorage` directly

---

## API Service Modules — `src/api/`

- `authApi.ts` — `login(email, password)`, `register(email, password)` → `TokenResponse`
- `expenseApi.ts` — `getAll()`, `getById(id)`, `create(payload)`, `update(id, payload)`, `remove(id)`
- `categoryApi.ts` — `getAll()`, `getById(id)`, `create(payload)`, `update(id, payload)`, `remove(id)`
- `incomeApi.ts` — `getAll()`, `getById(id)`, `create(payload)`, `update(id, payload)`, `remove(id)`
- `incomeCategoryApi.ts` — `getAll()`, `getById(id)`, `create(payload)`, `update(id, payload)`, `remove(id)`
- `budgetApi.ts` — `getAll()`, `getByMonth(year, month)`, `create(payload)`, `update(id, payload)`, `remove(id)`
- `userApi.ts` — `getAll()`, `updateRole(id, role)`

All use the existing `axiosInstance.ts`. Pages import from these modules — never call `api.get/post/put/delete` directly.

---

## Axios Instance — `src/api/axiosInstance.ts`

- `baseURL`: `http://localhost:5001`
- Request interceptor: read `localStorage.getItem("token")`, if present attach as `Authorization: Bearer <token>` header
- Export as default

---

## Layouts

### AuthLayout — `src/layouts/AuthLayout.tsx`
- Antd `<Card>` centered on full-height page with `<Flex>`
- Contains `<Outlet />` for child pages
- Use: AuthFormPage (login/register)

### MainLayout — `src/layouts/MainLayout.tsx`
- Antd `<Layout>` with `<Header>` containing `<Menu>` (dark theme, horizontal)
- Menu items: **Dashboard** (`/`), and if `useAuth().role` is `"Admin"` or `"SuperAdmin"` also **Admin** (`/admin/categories`)
- Logout button using `useAuth().logout()`, redirects to `/login`
- `<Content>` with `<Outlet />`, `<Footer>` with copyright
- Uses `@ant-design/icons` for menu items

### AdminLayout — `src/layouts/AdminLayout.tsx`
- Antd `<Layout>` with `<Sider>` containing `<Menu>` (light theme, inline)
- Menu items: **Categories** (`/admin/categories`), **Income Categories** (`/admin/income-categories`), and if `useAuth().role` is `"SuperAdmin"` also **Users** (`/admin/users`), plus **Back to Dashboard** (`/`)
- `<Content>` with `<Outlet />`

---

## Pages

### AuthFormPage — `src/pages/auth/AuthFormPage.tsx`
- Single component for both login and register, receives `mode: 'login' | 'register'` prop
- Formik form with Yup validation (email format, password min 8 chars for register)
- Antd `<Form>`, `<Input>`, `<Input.Password>`, `<Button>`, `<Alert>`, `<Typography>`
- On success: calls `useAuth().login(token)`, redirects to `/`
- Link to toggle between login/register

### DashboardPage — `src/pages/DashboardPage.tsx`
- Tab shell with month `DatePicker` (picker="month") and antd `Tabs`
- Three tabs: **Expenses** (`/`), **Incomes** (`/incomes`), **Budgets** (`/budgets`)
- Fetches all incomes to compute `totalIncome` for selected month
- Passes `{ year, month, totalIncome, onDataChanged }` to tab children via `useOutletContext`
- Exports `DashboardContext` interface for typed outlet context

### ExpensesTab — `src/pages/expenses/ExpensesTab.tsx`
- Receives month context from DashboardPage via `useOutletContext<DashboardContext>()`
- Fetches all expenses + categories, filters by selected month client-side
- Shows `FinancialSummary` (Total Income / Total Spent / Remaining Available)
- Category filter via `Select`, "New Expense" button disabled when remaining ≤ 0
- Delete via `Modal.confirm()` + `extractErrorDetail` for error messages

### IncomesTab — `src/pages/income/IncomesTab.tsx`
- Fetches all incomes + income categories, filters by selected month
- Table: Date, Income Category (`Tag`), Amount, Actions
- Shows total income for month
- Delete warns about budget/expense dependencies; 409 errors shown via `extractErrorDetail`

### BudgetsTab — `src/pages/budgets/BudgetsTab.tsx`
- Fetches budgets via `budgetApi.getByMonth()` + all expenses for spent calculation
- Table: Category (`Tag`), Budget Amount, Spent, % Used (`Progress`), Actions
- Shows `FinancialSummary` (Total Income / Total Planned Budgets / Remaining To Allocate)
- "New Budget" disabled when remaining to allocate ≤ 0

### ExpenseFormPage — `src/pages/expenses/ExpenseFormPage.tsx`
- Used for both **add** (`/expenses/new`) and **edit** (`/expenses/:id/edit`)
- Detect mode from `useParams` — if `id` present, load existing expense
- Formik form with Yup validation (amount > 0, description required, date required, categoryId required)
- Antd `<Form>`, `<InputNumber>`, `<Input>`, `<DatePicker>` (dayjs), `<Select>`, `<Button>`, `<Spin>`, `<Alert>`
- Uses `expenseApi` and `categoryApi`; 409 errors displayed via `extractErrorDetail`
- On success: redirects to `/`

### IncomeFormPage — `src/pages/income/IncomeFormPage.tsx`
- Used for both **add** (`/incomes/new`) and **edit** (`/incomes/:id/edit`)
- Formik + Yup: amount > 0, date required, incomeCategoryId required
- Antd `<Form>`, `<InputNumber>`, `<DatePicker>`, `<Select>`, `<Button>`, `<Spin>`, `<Alert>`
- Uses `incomeApi` and `incomeCategoryApi`; 409 errors displayed via `extractErrorDetail`
- On success: redirects to `/incomes`

### BudgetFormPage — `src/pages/budgets/BudgetFormPage.tsx`
- Used for both **add** (`/budgets/new`) and **edit** (`/budgets/:id/edit`)
- Reads `?year=Y&month=M` query params for default values when creating
- Formik + Yup: categoryId required, year 2000–2100, month 1–12, amount > 0
- Antd `<Form>`, `<InputNumber>`, `<Select>` (month dropdown), `<Button>`, `<Spin>`, `<Alert>`
- Uses `budgetApi` and `categoryApi`; 409 errors displayed via `extractErrorDetail`
- On success: redirects to `/budgets`

### CategoriesPage — `src/pages/admin/categories/CategoriesPage.tsx`
- Fetches categories via `categoryApi.getAll()`
- Antd `<Table>` with columns: color swatch (`<Tag color>`), name, actions
- Delete via `Modal.confirm()` + `message.success/error`
- Button to add new category (`/admin/categories/new`)

### CategoryFormPage — `src/pages/admin/categories/CategoryFormPage.tsx`
- Used for both **add** and **edit** (detect from `useParams`)
- Formik + Yup (name required, color valid hex)
- Antd `<Form>`, `<Input>`, `<ColorPicker>`, `<Button>`, `<Typography.Text code>` for hex display
- Uses `categoryApi`
- On success: redirects to `/admin/categories`

### IncomeCategoriesPage — `src/pages/admin/income-categories/IncomeCategoriesPage.tsx`
- Fetches income categories via `incomeCategoryApi.getAll()`
- Antd `<Table>` with columns: color swatch (`<Tag color>`), name, actions
- Delete via `Modal.confirm()` + `extractErrorDetail` for 409 errors (incomes reference category)
- Button to add new income category (`/admin/income-categories/new`)

### IncomeCategoryFormPage — `src/pages/admin/income-categories/IncomeCategoryFormPage.tsx`
- Used for both **add** and **edit** (detect from `useParams`)
- Formik + Yup (name required, color valid hex)
- Antd `<Form>`, `<Input>`, `<ColorPicker>`, `<Button>`, `<Typography.Text code>` for hex display
- Uses `incomeCategoryApi`; errors displayed via `extractErrorDetail`
- On success: redirects to `/admin/income-categories`

### UsersPage — `src/pages/admin/users/UsersPage.tsx`
- SuperAdmin only — fetches users via `userApi.getAll()`
- Antd `<Table>` with columns: ID, email, role (`<Tag color>` mapped by role), actions
- SuperAdmin row shows dash (no action)
- Admin row: "Revoke Admin" danger button → `userApi.updateRole(id, 'User')`
- User row: "Make Admin" primary button → `userApi.updateRole(id, 'Admin')`
- Uses `message.success/error` for feedback

---

## Route Guards

### ProtectedRoute — `src/components/ProtectedRoute.tsx`
- Uses `useAuth().isAuthenticated`
- If not authenticated → `<Navigate to="/login" replace />`
- If authenticated → `<Outlet />`

### AdminRoute — `src/components/AdminRoute.tsx`
- Uses `useAuth()` — checks `isAuthenticated` and `role`
- If not authenticated → `<Navigate to="/login" replace />`
- If role is not `"Admin"` and not `"SuperAdmin"` → `<Navigate to="/" replace />`
- Otherwise → `<Outlet />`

---

## Routing — `src/App.tsx`

```
/login              AuthLayout → AuthFormPage (mode="login")
/register           AuthLayout → AuthFormPage (mode="register")
/                   ProtectedRoute → MainLayout → DashboardPage (tab shell)
  / (index)           ExpensesTab (default tab)
  /incomes            IncomesTab
  /budgets            BudgetsTab
/expenses/new       ProtectedRoute → MainLayout → ExpenseFormPage
/expenses/:id/edit  ProtectedRoute → MainLayout → ExpenseFormPage
/incomes/new        ProtectedRoute → MainLayout → IncomeFormPage
/incomes/:id/edit   ProtectedRoute → MainLayout → IncomeFormPage
/budgets/new        ProtectedRoute → MainLayout → BudgetFormPage
/budgets/:id/edit   ProtectedRoute → MainLayout → BudgetFormPage
/admin              ProtectedRoute → AdminRoute → AdminLayout
  /admin/categories              CategoriesPage
  /admin/categories/new          CategoryFormPage
  /admin/categories/:id/edit     CategoryFormPage
  /admin/income-categories       IncomeCategoriesPage
  /admin/income-categories/new   IncomeCategoryFormPage
  /admin/income-categories/:id/edit  IncomeCategoryFormPage
  /admin/users                   UsersPage
```

---

## Coding Conventions
- Components declared with `function` keyword: `export default function DashboardPage() {}`
- All other functions (handlers, helpers, callbacks) use arrow functions: `const handleSubmit = async () => {}`
- **Types:** Import shared types from `src/types/models.ts` — never define `Category`, `Expense`, `User`, or `TokenResponse` locally
- **Auth:** Use `useAuth()` hook from `src/context/AuthContext.tsx` — never read `localStorage` or parse JWT manually
- **API:** Use service modules from `src/api/` (`authApi`, `expenseApi`, `categoryApi`, `incomeApi`, `incomeCategoryApi`, `budgetApi`, `userApi`) — never call `api.get/post/put/delete` directly in page components
- **Error handling:** Use `extractErrorDetail()` from `src/utils/errorUtils.ts` to parse ProblemDetails 409/4xx responses — never display generic error messages when the server provides a specific `detail` string
- **UI components:** Use Ant Design components (`Table`, `Button`, `Form`, `Input`, `Select`, `Tag`, `Modal`, `message`, `Alert`, `Spin`, `Layout`, `Menu`, `Card`, `Typography`, etc.) — no raw HTML tables, inputs, buttons, or inline styles
- **Forms:** Use Formik with Yup validation schemas — Formik manages form state, Yup defines validation rules, antd `<Form>` / `<Form.Item>` handles layout and error display
- **Validation schemas:** Keep Yup schemas co-located in the same feature folder as the page that uses them (e.g., `pages/expenses/expenseSchema.ts` next to `ExpenseFormPage.tsx`) — never define schemas inline in page components
- **Feedback:** Use antd `message.success/error` for action feedback, `Modal.confirm` for destructive actions, `<Alert>` for persistent errors — never use browser `alert()` or `confirm()`
- TypeScript — no `any`, use the shared interfaces
- Indentation: 2 spaces, no tabs
- Single quotes for strings (except JSX attributes which use double quotes)
