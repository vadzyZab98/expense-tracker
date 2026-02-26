import { Outlet } from 'react-router-dom';

export default function AuthLayout() {
  return (
    <div style={{
      minHeight: '100vh',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      backgroundColor: '#f0f2f5',
    }}>
      <div style={{
        background: '#fff',
        borderRadius: '8px',
        boxShadow: '0 2px 16px rgba(0,0,0,0.12)',
        padding: '40px 36px',
        width: '100%',
        maxWidth: '400px',
      }}>
        <Outlet />
      </div>
    </div>
  );
}
