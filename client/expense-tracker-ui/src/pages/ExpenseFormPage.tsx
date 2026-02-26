import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import api from '../api/axiosInstance';

interface Category {
  id: number;
  name: string;
  color: string;
}

export default function ExpenseFormPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);

  const [amount, setAmount] = useState('');
  const [description, setDescription] = useState('');
  const [date, setDate] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadData = async () => {
      setFetching(true);
      try {
        const catRes = await api.get<Category[]>('/api/categories');
        setCategories(catRes.data);

        if (isEdit) {
          const expRes = await api.get<{
            id: number;
            amount: number;
            description: string;
            date: string;
            categoryId: number;
          }>(`/api/expenses/${id}`);
          const e = expRes.data;
          setAmount(String(e.amount));
          setDescription(e.description);
          setDate(e.date.slice(0, 10));
          setCategoryId(String(e.categoryId));
        } else if (catRes.data.length > 0) {
          setCategoryId(String(catRes.data[0].id));
        }
      } catch {
        setError('Failed to load data.');
      } finally {
        setFetching(false);
      }
    };
    loadData();
  }, [id, isEdit]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    const payload = {
      amount: parseFloat(amount),
      description,
      date: new Date(date).toISOString(),
      categoryId: Number(categoryId),
    };
    try {
      if (isEdit) {
        await api.put(`/api/expenses/${id}`, payload);
      } else {
        await api.post('/api/expenses', payload);
      }
      navigate('/');
    } catch {
      setError('Failed to save expense.');
    } finally {
      setLoading(false);
    }
  };

  if (fetching) return <div style={{ padding: '24px' }}>Loading...</div>;

  return (
    <div style={{ padding: '24px', maxWidth: '480px' }}>
      <h2 style={{ marginTop: 0 }}>{isEdit ? 'Edit Expense' : 'New Expense'}</h2>
      <form onSubmit={handleSubmit}>
        <div style={fieldStyle}>
          <label style={labelStyle}>Amount</label>
          <input
            type='number'
            min='0'
            step='0.01'
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            required
            style={inputStyle}
          />
        </div>
        <div style={fieldStyle}>
          <label style={labelStyle}>Description</label>
          <input
            type='text'
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            required
            style={inputStyle}
          />
        </div>
        <div style={fieldStyle}>
          <label style={labelStyle}>Date</label>
          <input
            type='date'
            value={date}
            onChange={(e) => setDate(e.target.value)}
            required
            style={inputStyle}
          />
        </div>
        <div style={fieldStyle}>
          <label style={labelStyle}>Category</label>
          <select
            value={categoryId}
            onChange={(e) => setCategoryId(e.target.value)}
            required
            style={inputStyle}
          >
            {categories.map((c) => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
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
            onClick={() => navigate('/')}
            style={{ padding: '9px 24px', backgroundColor: '#fff', border: '1px solid #d9d9d9', borderRadius: '4px', cursor: 'pointer', fontSize: '14px' }}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

const fieldStyle: React.CSSProperties = { marginBottom: '16px' };

const labelStyle: React.CSSProperties = { display: 'block', marginBottom: '6px', fontWeight: 500, fontSize: '14px' };

const inputStyle: React.CSSProperties = { width: '100%', padding: '8px 10px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #d9d9d9', fontSize: '14px' };
