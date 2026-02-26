import { useState, useEffect } from 'react';
import { Table, Tag, Button, Alert, Typography, message } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import { userApi } from '../../api/userApi';
import type { User } from '../../types/models';

const roleColorMap: Record<string, string> = {
  SuperAdmin: 'purple',
  Admin: 'blue',
  User: 'green',
};

export default function UsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchUsers = async () => {
    setLoading(true);
    setError('');
    try {
      setUsers(await userApi.getAll());
    } catch {
      setError('Failed to load users.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchUsers(); }, []);

  const handleRoleChange = async (userId: number, newRole: string) => {
    try {
      await userApi.updateRole(userId, newRole);
      setUsers((prev) => prev.map((u) => (u.id === userId ? { ...u, role: newRole as User['role'] } : u)));
      message.success('Role updated.');
    } catch {
      message.error('Failed to update role.');
    }
  };

  const columns: ColumnsType<User> = [
    {
      title: 'ID',
      dataIndex: 'id',
      key: 'id',
      width: 80,
    },
    {
      title: 'Email',
      dataIndex: 'email',
      key: 'email',
    },
    {
      title: 'Role',
      dataIndex: 'role',
      key: 'role',
      render: (role: string) => <Tag color={roleColorMap[role] ?? 'default'}>{role}</Tag>,
    },
    {
      title: 'Actions',
      key: 'actions',
      align: 'center',
      render: (_: unknown, record: User) => {
        if (record.role === 'SuperAdmin') return <Typography.Text type="secondary">—</Typography.Text>;
        return record.role === 'Admin' ? (
          <Button size="small" danger onClick={() => handleRoleChange(record.id, 'User')}>
            Revoke Admin
          </Button>
        ) : (
          <Button size="small" type="primary" onClick={() => handleRoleChange(record.id, 'Admin')}>
            Make Admin
          </Button>
        );
      },
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <Typography.Title level={4} style={{ marginBottom: 16 }}>Users</Typography.Title>

      {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 16 }} />}

      <Table<User>
        columns={columns}
        dataSource={users}
        rowKey="id"
        loading={loading}
        pagination={false}
        locale={{ emptyText: 'No users found.' }}
      />
    </div>
  );
}
