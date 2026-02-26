import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import api from '../../api/axiosInstance';

export default function CategoryFormPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);

  const [name, setName] = useState('');
  const [color, setColor] = useState('#1677ff');
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(isEdit);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!isEdit) return;
    const load = async () => {
      try {
        const res = await api.get<{ id: number; name: string; color: string }>(`/api/categories/${id}`);
        setName(res.data.name);
        setColor(res.data.color);
      } catch {
        setError('Failed to load category.');
      } finally {
        setFetching(false);
      }
    };
    load();
  }, [id, isEdit]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      if (isEdit) {
        await api.put(`/api/categories/${id}`, { name, color });
      } else {
        await api.post('/api/categories', { name, color });
      }
      navigate('/admin/categories');
    } catch {
      setError('Failed to save category.');
    } finally {
      setLoading(false);
    }
  };

  if (fetching) return <div style={{ padding: '24px' }}>Loading...</div>;

  return (
    <div style={{ padding: '24px', maxWidth: '400px' }}>
      <h2 style={{ marginTop: 0 }}>{isEdit ? 'Edit Category' : 'New Category'}</h2>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '16px' }}>
          <label style={labelStyle}>Name</label>
          <input
            type='text'
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
            style={inputStyle}
          />
        </div>
        <div style={{ marginBottom: '24px' }}>
          <label style={labelStyle}>Color</label>
          <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
            <input
              type='color'
              value={color}
              onChange={(e) => setColor(e.target.value)}
              style={{ width: '48px', height: '36px', padding: '2px', border: '1px solid #d9d9d9', borderRadius: '4px', cursor: 'pointer' }}
            />
            <span style={{ fontSize: '14px', color: '#555', fontFamily: 'monospace' }}>{color}</span>
          </div>
        </div>
        {error && <p style={{ color: '#ff4d4f', marginBottom: '16px', fontSize: '14px' }}>{error}</p>}
        <div style={{ display: 'flex', gap: '12px' }}>
          <button
            type='submit'
            disabled={loading}
            style={{ padding: '9px 24px', backgroundColor: '#1677ff', color: '#fff', border: 'none', borderRadius: '4px', cursor: loading ? 'not-allowed' : 'pointer', opacity: loading ? 0.7 : 1, fontSize: '14px' }}
          >
            {loading ? 'Saving...' : isEdit ? 'Update' : 'Create'}
          </button>
          <button
            type='button'
            onClick={() => navigate('/admin/categories')}
            style={{ padding: '9px 24px', backgroundColor: '#fff', border: '1px solid #d9d9d9', borderRadius: '4px', cursor: 'pointer', fontSize: '14px' }}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

const labelStyle: React.CSSProperties = { display: 'block', marginBottom: '6px', fontWeight: 500, fontSize: '14px' };

const inputStyle: React.CSSProperties = { width: '100%', padding: '8px 10px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #d9d9d9', fontSize: '14px' };
