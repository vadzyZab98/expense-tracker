import { Outlet, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '@/context/AuthContext';
import styles from './MainLayout.module.css';

export default function MainLayout() {
  const navigate = useNavigate();
  const { isAdmin, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="flex min-h-screen flex-col">
      <nav className={styles.nav}>
        <span className={styles.brand}>Expense Tracker</span>
        <Link to="/" className={styles.navLink}>
          Dashboard
        </Link>
        {isAdmin && (
          <Link to="/admin/categories" className={styles.navLink}>
            Admin
          </Link>
        )}
        <button type="button" onClick={handleLogout} className={styles.logoutBtn}>
          Logout
        </button>
      </nav>
      <main className="flex-1">
        <Outlet />
      </main>
    </div>
  );
}
