import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import classnames from 'classnames';
import { format, parseISO } from 'date-fns';
import api from '@/api/axiosInstance';
import { useExpense, useCategories } from '@/hooks/useData';
import { expenseSchema } from '@/validation/schemas';
import type { ExpenseFormData } from '@/validation/schemas';
import type { ExpensePayload } from '@/types';
import styles from './ExpenseFormPage.module.css';

export default function ExpenseFormPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);

  const { expense, isLoading: expenseLoading } = useExpense(id);
  const { categories, isLoading: catLoading } = useCategories();
  const [serverError, setServerError] = useState('');

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<ExpenseFormData>({
    resolver: yupResolver(expenseSchema),
  });

  useEffect(() => {
    if (isEdit && expense) {
      reset({
        amount: expense.amount,
        description: expense.description,
        date: format(parseISO(expense.date), 'yyyy-MM-dd'),
        categoryId: expense.categoryId,
      });
    } else if (!isEdit && categories.length > 0) {
      reset({
        amount: undefined,
        description: '',
        date: '',
        categoryId: categories[0].id,
      });
    }
  }, [isEdit, expense, categories, reset]);

  const onSubmit = async (data: ExpenseFormData) => {
    setServerError('');
    const payload: ExpensePayload = {
      amount: data.amount,
      description: data.description,
      date: new Date(data.date).toISOString(),
      categoryId: data.categoryId,
    };
    try {
      if (isEdit) {
        await api.put(`/api/expenses/${id}`, payload);
      } else {
        await api.post('/api/expenses', payload);
      }
      navigate('/');
    } catch {
      setServerError('Failed to save expense.');
    }
  };

  if (expenseLoading || catLoading) {
    return <div className="p-6">Loading...</div>;
  }

  return (
    <div className={styles.container}>
      <h2 className={styles.heading}>{isEdit ? 'Edit Expense' : 'New Expense'}</h2>
      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        <div className={styles.field}>
          <label className={styles.label}>Amount</label>
          <input
            type="number"
            min="0"
            step="0.01"
            {...register('amount')}
            className={classnames(styles.input, { [styles.inputError]: errors.amount })}
          />
          {errors.amount && <p className={styles.errorText}>{errors.amount.message}</p>}
        </div>
        <div className={styles.field}>
          <label className={styles.label}>Description</label>
          <input
            type="text"
            {...register('description')}
            className={classnames(styles.input, { [styles.inputError]: errors.description })}
          />
          {errors.description && (
            <p className={styles.errorText}>{errors.description.message}</p>
          )}
        </div>
        <div className={styles.field}>
          <label className={styles.label}>Date</label>
          <input
            type="date"
            {...register('date')}
            className={classnames(styles.input, { [styles.inputError]: errors.date })}
          />
          {errors.date && <p className={styles.errorText}>{errors.date.message}</p>}
        </div>
        <div className={styles.field}>
          <label className={styles.label}>Category</label>
          <select
            {...register('categoryId')}
            className={classnames(styles.input, { [styles.inputError]: errors.categoryId })}
          >
            {categories.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </select>
          {errors.categoryId && (
            <p className={styles.errorText}>{errors.categoryId.message}</p>
          )}
        </div>
        {serverError && <p className={styles.serverError}>{serverError}</p>}
        <div className={styles.actions}>
          <button type="submit" disabled={isSubmitting} className={styles.submitBtn}>
            {isSubmitting ? 'Saving...' : isEdit ? 'Update' : 'Create'}
          </button>
          <button type="button" onClick={() => navigate('/')} className={styles.cancelBtn}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
