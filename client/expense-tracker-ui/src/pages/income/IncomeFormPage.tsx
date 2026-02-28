import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Formik } from 'formik';
import { Form, InputNumber, DatePicker, Select, Button, Alert, Spin, Typography, Space } from 'antd';
import dayjs from 'dayjs';
import { incomeApi } from '../../api/incomeApi';
import { incomeCategoryApi } from '../../api/incomeCategoryApi';
import { extractErrorDetail } from '../../utils/errorUtils';
import { incomeSchema } from './incomeSchema';
import type { IncomeCategory } from '../../types/models';

interface IncomeFormValues {
  amount: number;
  date: string;
  incomeCategoryId: number;
}

export default function IncomeFormPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);

  const [incomeCategories, setIncomeCategories] = useState<IncomeCategory[]>([]);
  const [initialValues, setInitialValues] = useState<IncomeFormValues | null>(null);
  const [fetching, setFetching] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadData = async () => {
      setFetching(true);
      try {
        const cats = await incomeCategoryApi.getAll();
        setIncomeCategories(cats);

        if (isEdit) {
          const income = await incomeApi.getById(Number(id));
          setInitialValues({
            amount: income.amount,
            date: income.date.slice(0, 10),
            incomeCategoryId: income.incomeCategoryId,
          });
        } else {
          setInitialValues({
            amount: 0,
            date: dayjs().format('YYYY-MM-DD'),
            incomeCategoryId: cats.length > 0 ? cats[0].id : 0,
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
        {isEdit ? 'Edit Income' : 'New Income'}
      </Typography.Title>

      <Formik<IncomeFormValues>
        initialValues={initialValues}
        validationSchema={incomeSchema}
        enableReinitialize
        onSubmit={async (values, { setStatus }) => {
          try {
            const payload = {
              ...values,
              date: new Date(values.date).toISOString(),
            };
            if (isEdit) {
              await incomeApi.update(Number(id), payload);
            } else {
              await incomeApi.create(payload);
            }
            navigate('/incomes');
          } catch (err) {
            setStatus(extractErrorDetail(err));
          }
        }}
      >
        {({ values, errors, touched, status, isSubmitting, setFieldValue, setFieldTouched, handleSubmit }) => (
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
              label="Income Category"
              validateStatus={touched.incomeCategoryId && errors.incomeCategoryId ? 'error' : undefined}
              help={touched.incomeCategoryId && errors.incomeCategoryId}
            >
              <Select
                value={values.incomeCategoryId}
                onChange={(val) => setFieldValue('incomeCategoryId', val)}
                onBlur={() => setFieldTouched('incomeCategoryId', true)}
                options={incomeCategories.map((c) => ({ label: c.name, value: c.id }))}
              />
            </Form.Item>

            {status && <Alert message={status} type="error" showIcon style={{ marginBottom: 16 }} />}
            {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 16 }} />}

            <Form.Item>
              <Space>
                <Button type="primary" htmlType="submit" loading={isSubmitting}>
                  {isEdit ? 'Update' : 'Create'}
                </Button>
                <Button onClick={() => navigate('/incomes')}>Cancel</Button>
              </Space>
            </Form.Item>
          </Form>
        )}
      </Formik>
    </div>
  );
}
