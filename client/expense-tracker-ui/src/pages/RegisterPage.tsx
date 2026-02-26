import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../api/axiosInstance';

export default function RegisterPage() {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const { data } = await api.post<{ token: string }>('/api/auth/register', { email, password });
      localStorage.setItem('token', data.token);
      navigate('/');
    } catch {
      setError('Registration failed. The email may already be in use.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <h2 style={{ marginTop: 0, marginBottom: '24px', textAlign: 'center' }}>Create Account</h2>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '16px' }}>
          <label style={{ display: 'block', marginBottom: '6px', fontWeight: 500 }}>Email</label>
          <input
            type='email'
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            style={{ width: '100%', padding: '8px 10px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #d9d9d9', fontSize: '14px' }}
          />
        </div>
        <div style={{ marginBottom: '20px' }}>
          <label style={{ display: 'block', marginBottom: '6px', fontWeight: 500 }}>Password</label>
          <input
            type='password'
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            style={{ width: '100%', padding: '8px 10px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #d9d9d9', fontSize: '14px' }}
          />
        </div>
        {error && <p style={{ color: '#ff4d4f', marginBottom: '16px', fontSize: '14px' }}>{error}</p>}
        <button
          type='submit'
          disabled={loading}
          style={{ width: '100%', padding: '10px', backgroundColor: '#1677ff', color: '#fff', border: 'none', borderRadius: '4px', fontSize: '15px', cursor: loading ? 'not-allowed' : 'pointer', opacity: loading ? 0.7 : 1 }}
        >
          {loading ? 'Creating account...' : 'Register'}
        </button>
      </form>
      <p style={{ textAlign: 'center', marginTop: '20px', fontSize: '14px' }}>
        Already have an account? <Link to='/login'>Sign in</Link>
      </p>
    </>
  );
}
