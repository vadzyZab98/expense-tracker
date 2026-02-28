import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Formik } from 'formik';
import { Form, Input, ColorPicker, Button, Alert, Spin, Typography, Space } from 'antd';
import { incomeCategoryApi } from '../../../api/incomeCategoryApi';
import { extractErrorDetail } from '../../../utils/errorUtils';
import { incomeCategorySchema } from './incomeCategorySchema';

interface IncomeCategoryFormValues {
  name: string;
  color: string;
}

export default function IncomeCategoryFormPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);

  const [initialValues, setInitialValues] = useState<IncomeCategoryFormValues | null>(isEdit ? null : { name: '', color: '#52c41a' });
  const [fetching, setFetching] = useState(isEdit);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!isEdit) return;
    const load = async () => {
      try {
        const cat = await incomeCategoryApi.getById(Number(id));
        setInitialValues({ name: cat.name, color: cat.color });
      } catch {
        setError('Failed to load income category.');
      } finally {
        setFetching(false);
      }
    };
    load();
  }, [id, isEdit]);

  if (fetching || !initialValues) {
    return <div style={{ padding: 24, textAlign: 'center' }}><Spin size="large" /></div>;
  }

  return (
    <div style={{ padding: 24, maxWidth: 400 }}>
      <Typography.Title level={4} style={{ marginTop: 0 }}>
        {isEdit ? 'Edit Income Category' : 'New Income Category'}
      </Typography.Title>

      <Formik<IncomeCategoryFormValues>
        initialValues={initialValues}
        validationSchema={incomeCategorySchema}
        enableReinitialize
        onSubmit={async (values, { setStatus }) => {
          try {
            if (isEdit) {
              await incomeCategoryApi.update(Number(id), values);
            } else {
              await incomeCategoryApi.create(values);
            }
            navigate('/admin/income-categories');
          } catch (err) {
            setStatus(extractErrorDetail(err));
          }
        }}
      >
        {({ values, errors, touched, status, isSubmitting, setFieldValue, handleChange, handleBlur, handleSubmit }) => (
          <Form layout="vertical" onFinish={handleSubmit}>
            <Form.Item
              label="Name"
              validateStatus={touched.name && errors.name ? 'error' : undefined}
              help={touched.name && errors.name}
            >
              <Input
                name="name"
                value={values.name}
                onChange={handleChange}
                onBlur={handleBlur}
              />
            </Form.Item>

            <Form.Item
              label="Color"
              validateStatus={touched.color && errors.color ? 'error' : undefined}
              help={touched.color && errors.color}
            >
              <Space>
                <ColorPicker
                  value={values.color}
                  disabledAlpha
                  onChange={(color) => {
                    const hex = color.toHexString().slice(0, 7);
                    setFieldValue('color', hex);
                  }}
                />
                <Typography.Text code>{values.color}</Typography.Text>
              </Space>
            </Form.Item>

            {status && <Alert message={status} type="error" showIcon style={{ marginBottom: 16 }} />}
            {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 16 }} />}

            <Form.Item>
              <Space>
                <Button type="primary" htmlType="submit" loading={isSubmitting}>
                  {isEdit ? 'Update' : 'Create'}
                </Button>
                <Button onClick={() => navigate('/admin/income-categories')}>Cancel</Button>
              </Space>
            </Form.Item>
          </Form>
        )}
      </Formik>
    </div>
  );
}
