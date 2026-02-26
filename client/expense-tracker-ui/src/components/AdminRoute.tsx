import { Navigate, Outlet } from 'react-router-dom';

export default function AdminRoute() {
  const token = localStorage.getItem('token');
  if (!token) {
    return <Navigate to='/login' replace />;
  }
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    if (payload.role !== 'Admin') {
      return <Navigate to='/' replace />;
    }
  } catch {
    return <Navigate to='/login' replace />;
  }
  return <Outlet />;
}
