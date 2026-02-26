import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Formik } from 'formik';
import * as Yup from 'yup';
import { Form, Input, InputNumber, DatePicker, Select, Button, Alert, Spin, Typography, Space } from 'antd';
import dayjs from 'dayjs';
import { expenseApi } from '../api/expenseApi';
import { categoryApi } from '../api/categoryApi';
import type { Category } from '../types/models';

export const expenseSchema = Yup.object({
  amount: Yup.number().min(0.01, 'Amount must be greater than 0').required('Amount is required'),
  description: Yup.string().required('Description is required'),
  date: Yup.string().required('Date is required'),
  categoryId: Yup.number().min(1, 'Category is required').required('Category is required'),
});

interface ExpenseFormValues {
  amount: number;
  description: string;
  date: string;
  categoryId: number;
}

export default function ExpenseFormPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);

  const [categories, setCategories] = useState<Category[]>([]);
  const [initialValues, setInitialValues] = useState<ExpenseFormValues | null>(null);
  const [fetching, setFetching] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadData = async () => {
      setFetching(true);
      try {
        const cats = await categoryApi.getAll();
        setCategories(cats);

        if (isEdit) {
          const expense = await expenseApi.getById(Number(id));
          setInitialValues({
            amount: expense.amount,
            description: expense.description,
            date: expense.date.slice(0, 10),
            categoryId: expense.categoryId,
          });
        } else {
          setInitialValues({
            amount: 0,
            description: '',
            date: dayjs().format('YYYY-MM-DD'),
            categoryId: cats.length > 0 ? cats[0].id : 0,
          });
        }
      } catch {
        setError('Failed to load data.');
      } finally {
        setFetching(false);
      }
    };
    loadData();
  }, [id, isEdit]);

  if (fetching || !initialValues) {
    return <div style={{ padding: 24, textAlign: 'center' }}><Spin size="large" /></div>;
  }

  return (
    <div style={{ padding: 24, maxWidth: 480 }}>
      <Typography.Title level={4} style={{ marginTop: 0 }}>
        {isEdit ? 'Edit Expense' : 'New Expense'}
      </Typography.Title>

      <Formik<ExpenseFormValues>
        initialValues={initialValues}
        validationSchema={expenseSchema}
        enableReinitialize
        onSubmit={async (values, { setStatus }) => {
          try {
            const payload = {
              ...values,
              date: new Date(values.date).toISOString(),
            };
            if (isEdit) {
              await expenseApi.update(Number(id), payload);
            } else {
              await expenseApi.create(payload);
            }
            navigate('/');
          } catch {
            setStatus('Failed to save expense.');
          }
        }}
      >
        {({ values, errors, touched, status, isSubmitting, setFieldValue, setFieldTouched, handleChange, handleBlur, handleSubmit }) => (
          <Form layout="vertical" onFinish={handleSubmit}>
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

            <Form.Item
              label="Description"
              validateStatus={touched.description && errors.description ? 'error' : undefined}
              help={touched.description && errors.description}
            >
              <Input
                name="description"
                value={values.description}
                onChange={handleChange}
                onBlur={handleBlur}
              />
            </Form.Item>

            <Form.Item
              label="Date"
              validateStatus={touched.date && errors.date ? 'error' : undefined}
              help={touched.date && errors.date}
            >
              <DatePicker
                value={values.date ? dayjs(values.date) : null}
                onChange={(d) => setFieldValue('date', d ? d.format('YYYY-MM-DD') : '')}
                onBlur={() => setFieldTouched('date', true)}
                style={{ width: '100%' }}
              />
            </Form.Item>

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

            {status && <Alert message={status} type="error" showIcon style={{ marginBottom: 16 }} />}
            {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 16 }} />}

            <Form.Item>
              <Space>
                <Button type="primary" htmlType="submit" loading={isSubmitting}>
                  {isEdit ? 'Update' : 'Create'}
                </Button>
                <Button onClick={() => navigate('/')}>Cancel</Button>
              </Space>
            </Form.Item>
          </Form>
        )}
      </Formik>
    </div>
  );
}
