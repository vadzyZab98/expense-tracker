import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { Layout, Menu } from 'antd';
import { AppstoreOutlined, TeamOutlined, ArrowLeftOutlined, DollarOutlined } from '@ant-design/icons';
import { useAuth } from '../context/AuthContext';

const { Sider, Content } = Layout;

export default function AdminLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const { role } = useAuth();

  const menuItems = [
    { key: '/admin/categories', icon: <AppstoreOutlined />, label: 'Categories' },
    { key: '/admin/income-categories', icon: <DollarOutlined />, label: 'Income Categories' },
    ...(role === 'SuperAdmin'
      ? [{ key: '/admin/users', icon: <TeamOutlined />, label: 'Users' }]
      : []),
    { type: 'divider' as const },
    { key: '/', icon: <ArrowLeftOutlined />, label: 'Back to Dashboard' },
  ];

  return (
    <Layout style={{ minHeight: 'calc(100vh - 64px)' }}>
      <Sider width={200} theme="light" style={{ borderRight: '1px solid #f0f0f0' }}>
        <Menu
          mode="inline"
          selectedKeys={[location.pathname]}
          items={menuItems}
          onClick={({ key }) => navigate(key)}
          style={{ height: '100%', paddingTop: 16 }}
        />
      </Sider>
      <Content style={{ overflow: 'auto' }}>
        <Outlet />
      </Content>
    </Layout>
  );
}
