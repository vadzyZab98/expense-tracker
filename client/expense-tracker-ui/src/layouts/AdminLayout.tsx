import { Outlet, NavLink, Link } from 'react-router-dom';

const getRole = (): string | null => {
  const token = localStorage.getItem('token');
  if (!token) return null;
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.role ?? payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? null;
  } catch {
    return null;
  }
};

const navLinkStyle = ({ isActive }: { isActive: boolean }): React.CSSProperties => ({
  display: 'block',
  padding: '10px 20px',
  color: isActive ? '#1677ff' : '#333',
  textDecoration: 'none',
  backgroundColor: isActive ? '#e6f4ff' : 'transparent',
  fontWeight: isActive ? 600 : 400,
  fontSize: '14px',
  borderLeft: isActive ? '3px solid #1677ff' : '3px solid transparent',
});

export default function AdminLayout() {
  const role = getRole();

  return (
    <div style={{ display: 'flex', minHeight: 'calc(100vh - 56px)' }}>
      <aside style={{
        width: '200px',
        backgroundColor: '#f5f5f5',
        borderRight: '1px solid #e8e8e8',
        padding: '24px 0',
        flexShrink: 0,
      }}>
        <p style={{ padding: '0 20px', fontWeight: 600, fontSize: '12px', color: '#888', textTransform: 'uppercase', marginBottom: '8px' }}>Admin</p>
        <NavLink to='/admin/categories' style={navLinkStyle}>
          Categories
        </NavLink>
        {role === 'SuperAdmin' && (
          <NavLink to='/admin/users' style={navLinkStyle}>
            Users
          </NavLink>
        )}
        <div style={{ marginTop: '24px', padding: '0 20px' }}>
          <Link to='/' style={{ fontSize: '14px', color: '#1677ff', textDecoration: 'none' }}>
            ← Back to Dashboard
          </Link>
        </div>
      </aside>
      <div style={{ flex: 1, overflow: 'auto' }}>
        <Outlet />
      </div>
    </div>
  );
}
