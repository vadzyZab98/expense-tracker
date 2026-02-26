import { Outlet, NavLink } from 'react-router-dom';

export default function AdminLayout() {
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
        <NavLink
          to='/admin/categories'
          style={({ isActive }) => ({
            display: 'block',
            padding: '10px 20px',
            color: isActive ? '#1677ff' : '#333',
            textDecoration: 'none',
            backgroundColor: isActive ? '#e6f4ff' : 'transparent',
            fontWeight: isActive ? 600 : 400,
            fontSize: '14px',
            borderLeft: isActive ? '3px solid #1677ff' : '3px solid transparent',
          })}
        >
          Categories
        </NavLink>
      </aside>
      <div style={{ flex: 1, overflow: 'auto' }}>
        <Outlet />
      </div>
    </div>
  );
}
