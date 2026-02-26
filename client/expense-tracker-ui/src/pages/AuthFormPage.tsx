import { useNavigate, Link } from 'react-router-dom';
import { Formik } from 'formik';
import * as Yup from 'yup';
import { Form, Input, Button, Alert, Typography } from 'antd';
import { useAuth } from '../context/AuthContext';
import { authApi } from '../api/authApi';

interface AuthFormPageProps {
  mode: 'login' | 'register';
}

const loginSchema = Yup.object({
  email: Yup.string().email('Invalid email').required('Email is required'),
  password: Yup.string().required('Password is required'),
});

const registerSchema = Yup.object({
  email: Yup.string().email('Invalid email').required('Email is required'),
  password: Yup.string().min(8, 'Password must be at least 8 characters').required('Password is required'),
});

export default function AuthFormPage({ mode }: AuthFormPageProps) {
  const navigate = useNavigate();
  const { login } = useAuth();
  const isLogin = mode === 'login';

  return (
    <>
      <Typography.Title level={3} style={{ textAlign: 'center', marginTop: 0 }}>
        {isLogin ? 'Sign In' : 'Create Account'}
      </Typography.Title>

      <Formik
        initialValues={{ email: '', password: '' }}
        validationSchema={isLogin ? loginSchema : registerSchema}
        onSubmit={async (values, { setStatus }) => {
          try {
            const apiFn = isLogin ? authApi.login : authApi.register;
            const { token } = await apiFn(values.email, values.password);
            login(token);
            navigate('/');
          } catch {
            setStatus(isLogin ? 'Invalid email or password.' : 'Registration failed. The email may already be in use.');
          }
        }}
      >
        {({ values, errors, touched, status, isSubmitting, handleChange, handleBlur, handleSubmit }) => (
          <Form layout="vertical" onFinish={handleSubmit}>
            <Form.Item
              label="Email"
              validateStatus={touched.email && errors.email ? 'error' : undefined}
              help={touched.email && errors.email}
            >
              <Input
                type="email"
                name="email"
                value={values.email}
                onChange={handleChange}
                onBlur={handleBlur}
              />
            </Form.Item>

            <Form.Item
              label="Password"
              validateStatus={touched.password && errors.password ? 'error' : undefined}
              help={touched.password && errors.password}
            >
              <Input.Password
                name="password"
                value={values.password}
                onChange={handleChange}
                onBlur={handleBlur}
              />
            </Form.Item>

            {status && <Alert message={status} type="error" showIcon style={{ marginBottom: 16 }} />}

            <Form.Item>
              <Button type="primary" htmlType="submit" loading={isSubmitting} block>
                {isLogin ? 'Sign In' : 'Register'}
              </Button>
            </Form.Item>

            <Typography.Paragraph style={{ textAlign: 'center' }}>
              {isLogin ? (
                <>Don&apos;t have an account? <Link to="/register">Register</Link></>
              ) : (
                <>Already have an account? <Link to="/login">Sign in</Link></>
              )}
            </Typography.Paragraph>
          </Form>
        )}
      </Formik>
    </>
  );
}
