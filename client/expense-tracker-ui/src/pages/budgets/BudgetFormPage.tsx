import { useState, useEffect } from 'react';
import { useNavigate, useParams, useSearchParams } from 'react-router-dom';
import { Formik } from 'formik';
import { Form, InputNumber, Select, Button, Alert, Spin, Typography, Space } from 'antd';
import dayjs from 'dayjs';
import { budgetApi } from '../../api/budgetApi';
import { categoryApi } from '../../api/categoryApi';
import { extractErrorDetail } from '../../utils/errorUtils';
import { budgetSchema } from './budgetSchema';
import type { Category } from '../../types/models';

interface BudgetFormValues {
  categoryId: number;
  year: number;
  month: number;
  amount: number;
}

const monthOptions = [
  { value: 1, label: 'January' },
  { value: 2, label: 'February' },
  { value: 3, label: 'March' },
  { value: 4, label: 'April' },
  { value: 5, label: 'May' },
  { value: 6, label: 'June' },
  { value: 7, label: 'July' },
  { value: 8, label: 'August' },
  { value: 9, label: 'September' },
  { value: 10, label: 'October' },
  { value: 11, label: 'November' },
  { value: 12, label: 'December' },
];

export default function BudgetFormPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [searchParams] = useSearchParams();
  const isEdit = Boolean(id);

  const defaultYear = Number(searchParams.get('year')) || dayjs().year();
  const defaultMonth = Number(searchParams.get('month')) || dayjs().month() + 1;

  const [categories, setCategories] = useState<Category[]>([]);
  const [initialValues, setInitialValues] = useState<BudgetFormValues | null>(null);
  const [fetching, setFetching] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadData = async () => {
      setFetching(true);
      try {
        const cats = await categoryApi.getAll();
        setCategories(cats);

        if (isEdit) {
          const budgets = await budgetApi.getAll();
          const budget = budgets.find((b) => b.id === Number(id));
          if (!budget) {
            setError('Budget not found.');
            setFetching(false);
            return;
          }
          setInitialValues({
            categoryId: budget.categoryId,
            year: budget.year,
            month: budget.month,
            amount: budget.amount,
          });
        } else {
          setInitialValues({
            categoryId: cats.length > 0 ? cats[0].id : 0,
            year: defaultYear,
            month: defaultMonth,
            amount: 0,
          });
        }
      } catch {
        setError('Failed to load data.');
      } finally {
        setFetching(false);
      }
    };
    loadData();
  }, [id, isEdit, defaultYear, defaultMonth]);

  if (fetching || !initialValues) {
    return <div style={{ padding: 24, textAlign: 'center' }}><Spin size="large" /></div>;
  }

  return (
    <div style={{ padding: 24, maxWidth: 480 }}>
      <Typography.Title level={4} style={{ marginTop: 0 }}>
        {isEdit ? 'Edit Budget' : 'New Budget'}
      </Typography.Title>

      <Formik<BudgetFormValues>
        initialValues={initialValues}
        validationSchema={budgetSchema}
        enableReinitialize
        onSubmit={async (values, { setStatus }) => {
          try {
            if (isEdit) {
              await budgetApi.update(Number(id), values);
            } else {
              await budgetApi.create(values);
            }
            navigate('/budgets');
          } catch (err) {
            setStatus(extractErrorDetail(err));
          }
        }}
      >
        {({ values, errors, touched, status, isSubmitting, setFieldValue, setFieldTouched, handleSubmit }) => (
          <Form layout="vertical" onFinish={handleSubmit}>
            <Form.Item
              label="Category"
              validateStatus={touched.categoryId && errors.categoryId ? 'error' : undefined}
              help={touched.categoryId && errors.categoryId}
            >
              <Select
                value={values.categoryId}
                onChange={(val) => setFieldValue('categoryId', val)}
                onBlur={() => setFieldTouched('categoryId', true)}
                options={categories.map((c) => ({ label: c.name, value: c.id }))}
              />
            </Form.Item>

            <Form.Item
              label="Year"
              validateStatus={touched.year && errors.year ? 'error' : undefined}
              help={touched.year && errors.year}
            >
              <InputNumber
                min={2000}
                max={2100}
                value={values.year}
                onChange={(val) => setFieldValue('year', val ?? dayjs().year())}
                onBlur={() => setFieldTouched('year', true)}
                style={{ width: '100%' }}
              />
            </Form.Item>

            <Form.Item
              label="Month"
              validateStatus={touched.month && errors.month ? 'error' : undefined}
              help={touched.month && errors.month}
            >
              <Select
                value={values.month}
                onChange={(val) => setFieldValue('month', val)}
                onBlur={() => setFieldTouched('month', true)}
                options={monthOptions}
              />
            </Form.Item>

            <Form.Item
              label="Amount"
              validateStatus={touched.amount && errors.amount ? 'error' : undefined}
              help={touched.amount && errors.amount}
            >
              <InputNumber
                min={0}
                step={0.01}
                value={values.amount}
                onChange={(val) => setFieldValue('amount', val ?? 0)}
                onBlur={() => setFieldTouched('amount', true)}
                style={{ width: '100%' }}
              />
            </Form.Item>

            {status && <Alert message={status} type="error" showIcon style={{ marginBottom: 16 }} />}
            {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 16 }} />}

            <Form.Item>
              <Space>
                <Button type="primary" htmlType="submit" loading={isSubmitting}>
                  {isEdit ? 'Update' : 'Create'}
                </Button>
                <Button onClick={() => navigate('/budgets')}>Cancel</Button>
              </Space>
            </Form.Item>
          </Form>
        )}
      </Formik>
    </div>
  );
}
