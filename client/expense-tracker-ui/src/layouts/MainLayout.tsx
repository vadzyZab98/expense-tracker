import { Outlet, Link, useNavigate } from 'react-router-dom';

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

export default function MainLayout() {
  const navigate = useNavigate();
  const role = getRole();

  const handleLogout = () => {
    localStorage.clear();
    navigate('/login');
  };

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <nav style={{
        display: 'flex',
        alignItems: 'center',
        gap: '24px',
        padding: '0 24px',
        height: '56px',
        backgroundColor: '#1677ff',
        color: '#fff',
      }}>
        <span style={{ fontWeight: 700, fontSize: '18px', marginRight: '8px' }}>Expense Tracker</span>
        <Link to='/' style={navLinkStyle}>Dashboard</Link>
        {role === 'Admin' && (
          <Link to='/admin/categories' style={navLinkStyle}>Admin</Link>
        )}
        <button
          onClick={handleLogout}
          style={{
            marginLeft: 'auto',
            padding: '6px 16px',
            backgroundColor: 'transparent',
            border: '1px solid rgba(255,255,255,0.7)',
            color: '#fff',
            borderRadius: '4px',
            cursor: 'pointer',
            fontSize: '14px',
          }}
        >
          Logout
        </button>
      </nav>
      <main style={{ flex: 1 }}>
        <Outlet />
      </main>
    </div>
  );
}

const navLinkStyle: React.CSSProperties = {
  color: '#fff',
  textDecoration: 'none',
  fontSize: '15px',
  opacity: 0.9,
};
