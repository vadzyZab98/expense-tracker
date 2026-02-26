import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import classnames from 'classnames';
import api from '@/api/axiosInstance';
import { useAuth } from '@/context/AuthContext';
import { loginSchema } from '@/validation/schemas';
import type { LoginFormData } from '@/validation/schemas';
import type { AuthResponse } from '@/types';
import { getErrorMessage } from '@/api/fetcher';
import styles from '../AuthForm.module.css';

export default function LoginPage() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [serverError, setServerError] = useState('');

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>({
    resolver: yupResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    setServerError('');
    try {
      const { data: resp } = await api.post<AuthResponse>('/api/auth/login', data);
      login(resp.token);
      navigate('/');
    } catch (err) {
      setServerError(getErrorMessage(err) || 'Invalid email or password.');
    }
  };

  return (
    <>
      <h2 className={styles.title}>Sign In</h2>
      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        <div className={styles.field}>
          <label className={styles.label}>Email</label>
          <input
            type="email"
            {...register('email')}
            className={classnames(styles.input, { [styles.inputError]: errors.email })}
          />
          {errors.email && <p className={styles.errorText}>{errors.email.message}</p>}
        </div>
        <div className="mb-5">
          <label className={styles.label}>Password</label>
          <input
            type="password"
            {...register('password')}
            className={classnames(styles.input, { [styles.inputError]: errors.password })}
          />
          {errors.password && <p className={styles.errorText}>{errors.password.message}</p>}
        </div>
        {serverError && <p className={styles.serverError}>{serverError}</p>}
        <button type="submit" disabled={isSubmitting} className={styles.submitBtn}>
          {isSubmitting ? 'Signing in...' : 'Sign In'}
        </button>
      </form>
      <p className={styles.linkText}>
        Don&apos;t have an account? <Link to="/register">Register</Link>
      </p>
    </>
  );
}
