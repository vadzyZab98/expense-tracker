import { Outlet, NavLink } from 'react-router-dom';
import classnames from 'classnames';
import styles from './AdminLayout.module.css';

export default function AdminLayout() {
  return (
    <div className={styles.wrapper}>
      <aside className={styles.sidebar}>
        <p className={styles.sidebarTitle}>Admin</p>
        <NavLink
          to="/admin/categories"
          className={({ isActive }) =>
            classnames(styles.sidebarLink, { [styles.sidebarLinkActive]: isActive })
          }
        >
          Categories
        </NavLink>
      </aside>
      <div className={styles.content}>
        <Outlet />
      </div>
    </div>
  );
}
