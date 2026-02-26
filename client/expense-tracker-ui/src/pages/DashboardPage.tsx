import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/axiosInstance';

interface Category {
  id: number;
  name: string;
  color: string;
}

interface Expense {
  id: number;
  userId: number;
  categoryId: number;
  category?: Category;
  amount: number;
  description: string;
  date: string;
}

export default function DashboardPage() {
  const navigate = useNavigate();
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [filterCategoryId, setFilterCategoryId] = useState<string>('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchData = async () => {
    setLoading(true);
    setError('');
    try {
      const [expensesRes, categoriesRes] = await Promise.all([
        api.get<Expense[]>('/api/expenses'),
        api.get<Category[]>('/api/categories'),
      ]);
      setExpenses(expensesRes.data);
      setCategories(categoriesRes.data);
    } catch {
      setError('Failed to load data.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchData(); }, []);

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this expense?')) return;
    try {
      await api.delete(`/api/expenses/${id}`);
      setExpenses((prev) => prev.filter((e) => e.id !== id));
    } catch {
      alert('Failed to delete expense.');
    }
  };

  const getCategoryById = (id: number) => categories.find((c) => c.id === id);

  const filtered = filterCategoryId
    ? expenses.filter((e) => e.categoryId === Number(filterCategoryId))
    : expenses;

  const total = filtered.reduce((sum, e) => sum + e.amount, 0);

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <h2 style={{ margin: 0 }}>My Expenses</h2>
        <button
          onClick={() => navigate('/expenses/new')}
          style={{ padding: '8px 18px', backgroundColor: '#1677ff', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '14px' }}
        >
          + New Expense
        </button>
      </div>

      <div style={{ display: 'flex', alignItems: 'center', gap: '16px', marginBottom: '16px' }}>
        <label style={{ fontWeight: 500 }}>Filter by category:</label>
        <select
          value={filterCategoryId}
          onChange={(e) => setFilterCategoryId(e.target.value)}
          style={{ padding: '6px 10px', borderRadius: '4px', border: '1px solid #d9d9d9', fontSize: '14px' }}
        >
          <option value=''>All</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
        </select>
        <span style={{ marginLeft: 'auto', fontWeight: 600, fontSize: '16px' }}>
          Total: ${total.toFixed(2)}
        </span>
      </div>

      {loading && <p>Loading...</p>}
      {error && <p style={{ color: '#ff4d4f' }}>{error}</p>}

      {!loading && !error && (
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '14px' }}>
          <thead>
            <tr style={{ backgroundColor: '#fafafa', textAlign: 'left' }}>
              <th style={thStyle}>Date</th>
              <th style={thStyle}>Description</th>
              <th style={thStyle}>Category</th>
              <th style={{ ...thStyle, textAlign: 'right' }}>Amount</th>
              <th style={{ ...thStyle, textAlign: 'center' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {filtered.length === 0 && (
              <tr><td colSpan={5} style={{ padding: '16px', textAlign: 'center', color: '#888' }}>No expenses found.</td></tr>
            )}
            {filtered.map((expense) => {
              const cat = expense.category ?? getCategoryById(expense.categoryId);
              return (
                <tr key={expense.id} style={{ borderBottom: '1px solid #f0f0f0' }}>
                  <td style={tdStyle}>{new Date(expense.date).toLocaleDateString()}</td>
                  <td style={tdStyle}>{expense.description}</td>
                  <td style={tdStyle}>
                    {cat ? (
                      <span style={{
                        backgroundColor: cat.color,
                        color: '#fff',
                        padding: '2px 10px',
                        borderRadius: '12px',
                        fontSize: '12px',
                        fontWeight: 500,
                      }}>
                        {cat.name}
                      </span>
                    ) : '—'}
                  </td>
                  <td style={{ ...tdStyle, textAlign: 'right' }}>${expense.amount.toFixed(2)}</td>
                  <td style={{ ...tdStyle, textAlign: 'center' }}>
                    <button
                      onClick={() => navigate(`/expenses/${expense.id}/edit`)}
                      style={editBtnStyle}
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => handleDelete(expense.id)}
                      style={deleteBtnStyle}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              );
            })}
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

const editBtnStyle: React.CSSProperties = {
  marginRight: '8px',
  padding: '4px 12px',
  backgroundColor: '#fff',
  border: '1px solid #1677ff',
  color: '#1677ff',
  borderRadius: '4px',
  cursor: 'pointer',
  fontSize: '13px',
};

const deleteBtnStyle: React.CSSProperties = {
  padding: '4px 12px',
  backgroundColor: '#fff',
  border: '1px solid #ff4d4f',
  color: '#ff4d4f',
  borderRadius: '4px',
  cursor: 'pointer',
  fontSize: '13px',
};
