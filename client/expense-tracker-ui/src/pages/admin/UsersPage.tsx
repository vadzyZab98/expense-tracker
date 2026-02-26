import { useState, useEffect } from 'react';
import api from '../../api/axiosInstance';

interface User {
  id: number;
  email: string;
  role: string;
}

export default function UsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchUsers = async () => {
    setLoading(true);
    setError('');
    try {
      const res = await api.get<User[]>('/api/users');
      setUsers(res.data);
    } catch {
      setError('Failed to load users.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchUsers(); }, []);

  const handleRoleChange = async (userId: number, newRole: string) => {
    try {
      await api.put(`/api/users/${userId}/role`, { role: newRole });
      setUsers((prev) =>
        prev.map((u) => (u.id === userId ? { ...u, role: newRole } : u))
      );
    } catch {
      alert('Failed to update role.');
    }
  };

  return (
    <div style={{ padding: '24px' }}>
      <h2 style={{ margin: '0 0 20px 0' }}>Users</h2>

      {loading && <p>Loading...</p>}
      {error && <p style={{ color: '#ff4d4f' }}>{error}</p>}

      {!loading && !error && (
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '14px' }}>
          <thead>
            <tr style={{ backgroundColor: '#fafafa', textAlign: 'left' }}>
              <th style={thStyle}>ID</th>
              <th style={thStyle}>Email</th>
              <th style={thStyle}>Role</th>
              <th style={{ ...thStyle, textAlign: 'center' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {users.length === 0 && (
              <tr>
                <td colSpan={4} style={{ padding: '16px', textAlign: 'center', color: '#888' }}>
                  No users found.
                </td>
              </tr>
            )}
            {users.map((user) => (
              <tr key={user.id} style={{ borderBottom: '1px solid #f0f0f0' }}>
                <td style={tdStyle}>{user.id}</td>
                <td style={tdStyle}>{user.email}</td>
                <td style={tdStyle}>
                  <span style={{
                    padding: '2px 10px',
                    borderRadius: '12px',
                    fontSize: '12px',
                    fontWeight: 500,
                    color: '#fff',
                    backgroundColor: user.role === 'SuperAdmin' ? '#722ed1' : user.role === 'Admin' ? '#1677ff' : '#52c41a',
                  }}>
                    {user.role}
                  </span>
                </td>
                <td style={{ ...tdStyle, textAlign: 'center' }}>
                  {user.role === 'SuperAdmin' ? (
                    <span style={{ color: '#888', fontSize: '13px' }}>—</span>
                  ) : user.role === 'Admin' ? (
                    <button
                      onClick={() => handleRoleChange(user.id, 'User')}
                      style={revokeBtnStyle}
                    >
                      Revoke Admin
                    </button>
                  ) : (
                    <button
                      onClick={() => handleRoleChange(user.id, 'Admin')}
                      style={assignBtnStyle}
                    >
                      Make Admin
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

const thStyle: React.CSSProperties = {
  padding: '10px 12px',
  borderBottom: '2px solid #f0f0f0',
  fontWeight: 600,
};

const tdStyle: React.CSSProperties = {
  padding: '10px 12px',
};

const assignBtnStyle: React.CSSProperties = {
  padding: '4px 12px',
  backgroundColor: '#fff',
  border: '1px solid #1677ff',
  color: '#1677ff',
  borderRadius: '4px',
  cursor: 'pointer',
  fontSize: '13px',
};

const revokeBtnStyle: React.CSSProperties = {
  padding: '4px 12px',
  backgroundColor: '#fff',
  border: '1px solid #ff4d4f',
  color: '#ff4d4f',
  borderRadius: '4px',
  cursor: 'pointer',
  fontSize: '13px',
};
