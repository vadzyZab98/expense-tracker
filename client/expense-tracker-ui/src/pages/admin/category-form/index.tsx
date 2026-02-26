import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import classnames from 'classnames';
import api from '@/api/axiosInstance';
import { useCategory } from '@/hooks/useData';
import { categorySchema } from '@/validation/schemas';
import type { CategoryFormData } from '@/validation/schemas';
import type { CategoryPayload } from '@/types';
import styles from './CategoryFormPage.module.css';

export default function CategoryFormPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);

  const { category, isLoading: catLoading } = useCategory(id);
  const [serverError, setServerError] = useState('');

  const {
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<CategoryFormData>({
    resolver: yupResolver(categorySchema),
    defaultValues: { name: '', color: '#1677ff' },
  });

  const colorValue = watch('color');

  useEffect(() => {
    if (isEdit && category) {
      reset({ name: category.name, color: category.color });
    }
  }, [isEdit, category, reset]);

  const onSubmit = async (data: CategoryFormData) => {
    setServerError('');
    const payload: CategoryPayload = { name: data.name, color: data.color };
    try {
      if (isEdit) {
        await api.put(`/api/categories/${id}`, payload);
      } else {
        await api.post('/api/categories', payload);
      }
      navigate('/admin/categories');
    } catch {
      setServerError('Failed to save category.');
    }
  };

  if (catLoading) {
    return <div className="p-6">Loading...</div>;
  }

  return (
    <div className={styles.container}>
      <h2 className={styles.heading}>{isEdit ? 'Edit Category' : 'New Category'}</h2>
      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        <div className={styles.field}>
          <label className={styles.label}>Name</label>
          <input
            type="text"
            {...register('name')}
            className={classnames(styles.input, { [styles.inputError]: errors.name })}
          />
          {errors.name && <p className={styles.errorText}>{errors.name.message}</p>}
        </div>
        <div className={styles.fieldLast}>
          <label className={styles.label}>Color</label>
          <div className={styles.colorRow}>
            <input type="color" {...register('color')} className={styles.colorInput} />
            <span className={styles.colorHex}>{colorValue}</span>
          </div>
          {errors.color && <p className={styles.errorText}>{errors.color.message}</p>}
        </div>
        {serverError && <p className={styles.serverError}>{serverError}</p>}
        <div className={styles.actions}>
          <button type="submit" disabled={isSubmitting} className={styles.submitBtn}>
            {isSubmitting ? 'Saving...' : isEdit ? 'Update' : 'Create'}
          </button>
          <button
            type="button"
            onClick={() => navigate('/admin/categories')}
            className={styles.cancelBtn}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
