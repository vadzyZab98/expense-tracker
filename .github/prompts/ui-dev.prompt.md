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
    models.ts                 ✅ done  — Category, Expense, User, TokenResponse
  context/
    AuthContext.tsx            ✅ done  — AuthProvider + useAuth() hook
  api/
    axiosInstance.ts           ✅ done  — Axios instance with JWT interceptor
    authApi.ts                ✅ done  — login(), register()
    expenseApi.ts             ✅ done  — getAll(), getById(), create(), update(), remove()
    categoryApi.ts            ✅ done  — getAll(), getById(), create(), update(), remove()
    userApi.ts                ✅ done  — getAll(), updateRole()
  layouts/
    AuthLayout.tsx            ✅ done  — antd Card centered
    MainLayout.tsx            ✅ done  — antd Layout + Header Menu + Footer
    AdminLayout.tsx           ✅ done  — antd Sider + Menu
  pages/
    AuthFormPage.tsx          ✅ done  — Merged login/register (Formik + antd)
    DashboardPage.tsx         ✅ done  — antd Table + Select + Tag
    ExpenseFormPage.tsx       ✅ done  — Formik + antd Form
    admin/
      CategoriesPage.tsx      ✅ done  — antd Table
      CategoryFormPage.tsx    ✅ done  — Formik + antd Form + ColorPicker
      UsersPage.tsx           ✅ done  — antd Table + Tag
  components/
    ProtectedRoute.tsx        ✅ done  — uses useAuth()
    AdminRoute.tsx            ✅ done  — uses useAuth()
  App.tsx                     ✅ done  — Routes + ConfigProvider
  main.tsx                    ✅ done  — AuthProvider wrapper
```

---

## TypeScript Interfaces

All shared types are defined in `src/types/models.ts`. Import from there — never define types locally in page files.

```typescript
import type { Category, Expense, User, TokenResponse } from '../types/models';
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
- Menu items: **Categories** (`/admin/categories`), and if `useAuth().role` is `"SuperAdmin"` also **Users** (`/admin/users`), plus **Back to Dashboard** (`/`)
- `<Content>` with `<Outlet />`

---

## Pages

### AuthFormPage — `src/pages/AuthFormPage.tsx`
- Single component for both login and register, receives `mode: 'login' | 'register'` prop
- Formik form with Yup validation (email format, password min 8 chars for register)
- Antd `<Form>`, `<Input>`, `<Input.Password>`, `<Button>`, `<Alert>`, `<Typography>`
- On success: calls `useAuth().login(token)`, redirects to `/`
- Link to toggle between login/register

### DashboardPage — `src/pages/DashboardPage.tsx`
- Fetches expenses via `expenseApi.getAll()` and categories via `categoryApi.getAll()`
- Antd `<Table>` with columns: date, description, category (`<Tag color>`), amount, actions
- Filter by category using antd `<Select allowClear>`
- Shows total sum of filtered expenses
- Delete via `Modal.confirm()` + `message.success/error`
- Edit/New buttons using antd `<Button>` with `@ant-design/icons`

### ExpenseFormPage — `src/pages/ExpenseFormPage.tsx`
- Used for both **add** (`/expenses/new`) and **edit** (`/expenses/:id/edit`)
- Detect mode from `useParams` — if `id` present, load existing expense
- Formik form with Yup validation (amount > 0, description required, date required, categoryId required)
- Antd `<Form>`, `<InputNumber>`, `<Input>`, `<DatePicker>` (dayjs), `<Select>`, `<Button>`, `<Spin>`, `<Alert>`
- Uses `expenseApi` and `categoryApi`
- On success: redirects to `/`

### CategoriesPage — `src/pages/admin/CategoriesPage.tsx`
- Fetches categories via `categoryApi.getAll()`
- Antd `<Table>` with columns: color swatch (`<Tag color>`), name, actions
- Delete via `Modal.confirm()` + `message.success/error`
- Button to add new category (`/admin/categories/new`)

### CategoryFormPage — `src/pages/admin/CategoryFormPage.tsx`
- Used for both **add** and **edit** (detect from `useParams`)
- Formik + Yup (name required, color valid hex)
- Antd `<Form>`, `<Input>`, `<ColorPicker>`, `<Button>`, `<Typography.Text code>` for hex display
- Uses `categoryApi`
- On success: redirects to `/admin/categories`

### UsersPage — `src/pages/admin/UsersPage.tsx`
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
/                   ProtectedRoute → MainLayout → DashboardPage
/expenses/new       ProtectedRoute → MainLayout → ExpenseFormPage
/expenses/:id/edit  ProtectedRoute → MainLayout → ExpenseFormPage
/admin              ProtectedRoute → AdminRoute → AdminLayout
  /admin/categories           CategoriesPage
  /admin/categories/new       CategoryFormPage
  /admin/categories/:id/edit  CategoryFormPage
  /admin/users                UsersPage
```

---

## Coding Conventions
- Components declared with `function` keyword: `export default function DashboardPage() {}`
- All other functions (handlers, helpers, callbacks) use arrow functions: `const handleSubmit = async () => {}`
- **Types:** Import shared types from `src/types/models.ts` — never define `Category`, `Expense`, `User`, or `TokenResponse` locally
- **Auth:** Use `useAuth()` hook from `src/context/AuthContext.tsx` — never read `localStorage` or parse JWT manually
- **API:** Use service modules from `src/api/` (`authApi`, `expenseApi`, `categoryApi`, `userApi`) — never call `api.get/post/put/delete` directly in page components
- **UI components:** Use Ant Design components (`Table`, `Button`, `Form`, `Input`, `Select`, `Tag`, `Modal`, `message`, `Alert`, `Spin`, `Layout`, `Menu`, `Card`, `Typography`, etc.) — no raw HTML tables, inputs, buttons, or inline styles
- **Forms:** Use Formik with Yup validation schemas — Formik manages form state, Yup defines validation rules, antd `<Form>` / `<Form.Item>` handles layout and error display
- **Feedback:** Use antd `message.success/error` for action feedback, `Modal.confirm` for destructive actions, `<Alert>` for persistent errors — never use browser `alert()` or `confirm()`
- TypeScript — no `any`, use the shared interfaces
- Indentation: 2 spaces, no tabs
- Single quotes for strings (except JSX attributes which use double quotes)
