import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../../api/axiosInstance';

interface Category {
  id: number;
  name: string;
  color: string;
}

export default function CategoriesPage() {
  const navigate = useNavigate();
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchCategories = async () => {
    setLoading(true);
    setError('');
    try {
      const res = await api.get<Category[]>('/api/categories');
      setCategories(res.data);
    } catch {
      setError('Failed to load categories.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchCategories(); }, []);

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this category?')) return;
    try {
      await api.delete(`/api/categories/${id}`);
      setCategories((prev) => prev.filter((c) => c.id !== id));
    } catch {
      alert('Failed to delete category.');
    }
  };

  return (
    <div style={{ padding: '24px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <h2 style={{ margin: 0 }}>Categories</h2>
        <button
          onClick={() => navigate('/admin/categories/new')}
          style={{ padding: '8px 18px', backgroundColor: '#1677ff', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '14px' }}
        >
          + New Category
        </button>
      </div>

      {loading && <p>Loading...</p>}
      {error && <p style={{ color: '#ff4d4f' }}>{error}</p>}

      {!loading && !error && (
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '14px' }}>
          <thead>
            <tr style={{ backgroundColor: '#fafafa', textAlign: 'left' }}>
              <th style={thStyle}>Color</th>
              <th style={thStyle}>Name</th>
              <th style={{ ...thStyle, textAlign: 'center' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {categories.length === 0 && (
              <tr><td colSpan={3} style={{ padding: '16px', textAlign: 'center', color: '#888' }}>No categories yet.</td></tr>
            )}
            {categories.map((cat) => (
              <tr key={cat.id} style={{ borderBottom: '1px solid #f0f0f0' }}>
                <td style={tdStyle}>
                  <span style={{
                    display: 'inline-block',
                    width: '24px',
                    height: '24px',
                    borderRadius: '4px',
                    backgroundColor: cat.color,
                    border: '1px solid rgba(0,0,0,0.1)',
                    verticalAlign: 'middle',
                  }} />
                </td>
                <td style={tdStyle}>{cat.name}</td>
                <td style={{ ...tdStyle, textAlign: 'center' }}>
                  <button
                    onClick={() => navigate(`/admin/categories/${cat.id}/edit`)}
                    style={editBtnStyle}
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(cat.id)}
                    style={deleteBtnStyle}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
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
