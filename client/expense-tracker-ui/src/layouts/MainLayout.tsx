import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { Layout, Menu, Button, Typography } from 'antd';
import { DashboardOutlined, SettingOutlined, LogoutOutlined } from '@ant-design/icons';
import { useAuth } from '../context/AuthContext';

const { Header, Content, Footer } = Layout;

export default function MainLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const { role, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const menuItems = [
    { key: '/', icon: <DashboardOutlined />, label: 'Dashboard' },
    ...((role === 'Admin' || role === 'SuperAdmin')
      ? [{ key: '/admin/categories', icon: <SettingOutlined />, label: 'Admin' }]
      : []),
  ];

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Header style={{ display: 'flex', alignItems: 'center', padding: '0 24px' }}>
        <Typography.Text strong style={{ color: '#fff', fontSize: 18, marginRight: 24, whiteSpace: 'nowrap' }}>
          Expense Tracker
        </Typography.Text>
        <Menu
          theme="dark"
          mode="horizontal"
          selectedKeys={[location.pathname]}
          items={menuItems}
          onClick={({ key }) => navigate(key)}
          style={{ flex: 1, minWidth: 0 }}
        />
        <Button type="text" icon={<LogoutOutlined />} onClick={handleLogout} style={{ color: '#fff' }}>
          Logout
        </Button>
      </Header>
      <Content style={{ flex: 1 }}>
        <Outlet />
      </Content>
      <Footer style={{ textAlign: 'center', color: '#888' }}>
        💰 Expense Tracker &copy; {new Date().getFullYear()}
      </Footer>
    </Layout>
  );
}
