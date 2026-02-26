import { Outlet } from 'react-router-dom';
import { Card, Flex } from 'antd';

export default function AuthLayout() {
  return (
    <Flex align="center" justify="center" style={{ minHeight: '100vh', backgroundColor: '#f0f2f5' }}>
      <Card style={{ width: '100%', maxWidth: 400 }}>
        <Outlet />
      </Card>
    </Flex>
  );
}
