import { Routes, Route } from 'react-router-dom';
import AuthLayout from './layouts/AuthLayout';
import MainLayout from './layouts/MainLayout';
import AdminLayout from './layouts/AdminLayout';
import ProtectedRoute from './components/ProtectedRoute';
import AdminRoute from './components/AdminRoute';
import AuthFormPage from './pages/AuthFormPage';
import DashboardPage from './pages/DashboardPage';
import ExpenseFormPage from './pages/ExpenseFormPage';
import CategoriesPage from './pages/admin/CategoriesPage';
import CategoryFormPage from './pages/admin/CategoryFormPage';
import UsersPage from './pages/admin/UsersPage';

export default function App() {
  return (
    <Routes>
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<AuthFormPage mode="login" />} />
        <Route path="/register" element={<AuthFormPage mode="register" />} />
      </Route>

      <Route element={<ProtectedRoute />}>
        <Route element={<MainLayout />}>
          <Route path="/" element={<DashboardPage />} />
          <Route path="/expenses/new" element={<ExpenseFormPage />} />
          <Route path="/expenses/:id/edit" element={<ExpenseFormPage />} />
        </Route>

        <Route element={<AdminRoute />}>
          <Route path="/admin" element={<AdminLayout />}>
            <Route path="categories" element={<CategoriesPage />} />
            <Route path="categories/new" element={<CategoryFormPage />} />
            <Route path="categories/:id/edit" element={<CategoryFormPage />} />
            <Route path="users" element={<UsersPage />} />
          </Route>
        </Route>
      </Route>
    </Routes>
  );
}
