import { Routes, Route } from 'react-router-dom';
import AuthLayout from './layouts/AuthLayout';
import MainLayout from './layouts/MainLayout';
import AdminLayout from './layouts/AdminLayout';
import ProtectedRoute from './components/ProtectedRoute';
import AdminRoute from './components/AdminRoute';
import AuthFormPage from './pages/auth/AuthFormPage';
import DashboardPage from './pages/DashboardPage';
import ExpensesTab from './pages/expenses/ExpensesTab';
import IncomesTab from './pages/income/IncomesTab';
import BudgetsTab from './pages/budgets/BudgetsTab';
import ExpenseFormPage from './pages/expenses/ExpenseFormPage';
import IncomeFormPage from './pages/income/IncomeFormPage';
import BudgetFormPage from './pages/budgets/BudgetFormPage';
import CategoriesPage from './pages/admin/categories/CategoriesPage';
import CategoryFormPage from './pages/admin/categories/CategoryFormPage';
import IncomeCategoriesPage from './pages/admin/income-categories/IncomeCategoriesPage';
import IncomeCategoryFormPage from './pages/admin/income-categories/IncomeCategoryFormPage';
import UsersPage from './pages/admin/users/UsersPage';

export default function App() {
  return (
    <Routes>
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<AuthFormPage mode="login" />} />
        <Route path="/register" element={<AuthFormPage mode="register" />} />
      </Route>

      <Route element={<ProtectedRoute />}>
        <Route element={<MainLayout />}>
          <Route element={<DashboardPage />}>
            <Route path="/" element={<ExpensesTab />} />
            <Route path="/incomes" element={<IncomesTab />} />
            <Route path="/budgets" element={<BudgetsTab />} />
          </Route>
          <Route path="/expenses/new" element={<ExpenseFormPage />} />
          <Route path="/expenses/:id/edit" element={<ExpenseFormPage />} />
          <Route path="/incomes/new" element={<IncomeFormPage />} />
          <Route path="/incomes/:id/edit" element={<IncomeFormPage />} />
          <Route path="/budgets/new" element={<BudgetFormPage />} />
          <Route path="/budgets/:id/edit" element={<BudgetFormPage />} />
        </Route>

        <Route element={<AdminRoute />}>
          <Route path="/admin" element={<AdminLayout />}>
            <Route path="categories" element={<CategoriesPage />} />
            <Route path="categories/new" element={<CategoryFormPage />} />
            <Route path="categories/:id/edit" element={<CategoryFormPage />} />
            <Route path="income-categories" element={<IncomeCategoriesPage />} />
            <Route path="income-categories/new" element={<IncomeCategoryFormPage />} />
            <Route path="income-categories/:id/edit" element={<IncomeCategoryFormPage />} />
            <Route path="users" element={<UsersPage />} />
          </Route>
        </Route>
      </Route>
    </Routes>
  );
}
