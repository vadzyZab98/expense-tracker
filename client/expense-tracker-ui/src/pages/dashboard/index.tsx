import { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { sumBy } from 'lodash';
import api from '@/api/axiosInstance';
import { useExpenses, useCategories } from '@/hooks/useData';
import { formatDate, formatCurrency } from '@/utils/helpers';
import styles from './DashboardPage.module.css';

export default function DashboardPage() {
  const navigate = useNavigate();
  const { expenses, isLoading: expLoading, isError: expError, mutate } = useExpenses();
  const { categories, isLoading: catLoading, isError: catError } = useCategories();
  const [filterCategoryId, setFilterCategoryId] = useState('');

  const isLoading = expLoading || catLoading;
  const isError = expError || catError;

  const filtered = useMemo(() => {
    if (!filterCategoryId) return expenses;
    return expenses.filter((e) => e.categoryId === Number(filterCategoryId));
  }, [expenses, filterCategoryId]);

  const total = useMemo(() => sumBy(filtered, 'amount'), [filtered]);

  const getCategoryById = (id: number) => categories.find((c) => c.id === id);

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this expense?')) return;
    try {
      await api.delete(`/api/expenses/${id}`);
      mutate();
    } catch {
      alert('Failed to delete expense.');
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h2 className={styles.heading}>My Expenses</h2>
        <button type="button" onClick={() => navigate('/expenses/new')} className={styles.addBtn}>
          + New Expense
        </button>
      </div>

      <div className={styles.filterRow}>
        <label className={styles.filterLabel}>Filter by category:</label>
        <select
          value={filterCategoryId}
          onChange={(e) => setFilterCategoryId(e.target.value)}
          className={styles.filterSelect}
        >
          <option value="">All</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>
        <span className={styles.total}>Total: {formatCurrency(total)}</span>
      </div>

      {isLoading && <p>Loading...</p>}
      {isError && <p className={styles.errorText}>Failed to load data.</p>}

      {!isLoading && !isError && (
        <table className={styles.table}>
          <thead>
            <tr className={styles.tableHead}>
              <th className={styles.th}>Date</th>
              <th className={styles.th}>Description</th>
              <th className={styles.th}>Category</th>
              <th className={styles.thRight}>Amount</th>
              <th className={styles.thCenter}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {filtered.length === 0 && (
              <tr>
                <td colSpan={5} className={styles.emptyRow}>
                  No expenses found.
                </td>
              </tr>
            )}
            {filtered.map((expense) => {
              const cat = expense.category ?? getCategoryById(expense.categoryId);
              return (
                <tr key={expense.id} className={styles.row}>
                  <td className={styles.td}>{formatDate(expense.date)}</td>
                  <td className={styles.td}>{expense.description}</td>
                  <td className={styles.td}>
                    {cat ? (
                      <span className={styles.badge} style={{ backgroundColor: cat.color }}>
                        {cat.name}
                      </span>
                    ) : (
                      '—'
                    )}
                  </td>
                  <td className={styles.tdRight}>{formatCurrency(expense.amount)}</td>
                  <td className={styles.tdCenter}>
                    <button
                      type="button"
                      onClick={() => navigate(`/expenses/${expense.id}/edit`)}
                      className={styles.editBtn}
                    >
                      Edit
                    </button>
                    <button
                      type="button"
                      onClick={() => handleDelete(expense.id)}
                      className={styles.deleteBtn}
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
