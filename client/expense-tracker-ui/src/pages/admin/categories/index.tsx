import { useNavigate } from 'react-router-dom';
import api from '@/api/axiosInstance';
import { getErrorMessage } from '@/api/fetcher';
import { useCategories } from '@/hooks/useData';
import styles from './CategoriesPage.module.css';

export default function CategoriesPage() {
  const navigate = useNavigate();
  const { categories, isLoading, isError, mutate } = useCategories();

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this category?')) return;
    try {
      await api.delete(`/api/categories/${id}`);
      mutate();
    } catch (err) {
      alert(getErrorMessage(err));
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h2 className={styles.heading}>Categories</h2>
        <button
          type="button"
          onClick={() => navigate('/admin/categories/new')}
          className={styles.addBtn}
        >
          + New Category
        </button>
      </div>

      {isLoading && <p>Loading...</p>}
      {isError && <p className={styles.errorText}>Failed to load categories.</p>}

      {!isLoading && !isError && (
        <table className={styles.table}>
          <thead>
            <tr className={styles.tableHead}>
              <th className={styles.th}>Color</th>
              <th className={styles.th}>Name</th>
              <th className={styles.thCenter}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {categories.length === 0 && (
              <tr>
                <td colSpan={3} className={styles.emptyRow}>
                  No categories yet.
                </td>
              </tr>
            )}
            {categories.map((cat) => (
              <tr key={cat.id} className={styles.row}>
                <td className={styles.td}>
                  <span
                    className={styles.colorSwatch}
                    style={{ backgroundColor: cat.color }}
                  />
                </td>
                <td className={styles.td}>{cat.name}</td>
                <td className={styles.tdCenter}>
                  <button
                    type="button"
                    onClick={() => navigate(`/admin/categories/${cat.id}/edit`)}
                    className={styles.editBtn}
                  >
                    Edit
                  </button>
                  <button
                    type="button"
                    onClick={() => handleDelete(cat.id)}
                    className={styles.deleteBtn}
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
