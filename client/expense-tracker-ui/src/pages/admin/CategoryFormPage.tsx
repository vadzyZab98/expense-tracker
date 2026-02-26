import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Formik } from 'formik';
import * as Yup from 'yup';
import { Form, Input, ColorPicker, Button, Alert, Spin, Typography, Space } from 'antd';
import { categoryApi } from '../../api/categoryApi';

export const categorySchema = Yup.object({
  name: Yup.string().required('Name is required'),
  color: Yup.string()
    .matches(/^#[0-9a-fA-F]{6}$/, 'Must be a valid hex color')
    .required('Color is required'),
});

interface CategoryFormValues {
  name: string;
  color: string;
}

export default function CategoryFormPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);

  const [initialValues, setInitialValues] = useState<CategoryFormValues | null>(isEdit ? null : { name: '', color: '#1677ff' });
  const [fetching, setFetching] = useState(isEdit);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!isEdit) return;
    const load = async () => {
      try {
        const cat = await categoryApi.getById(Number(id));
        setInitialValues({ name: cat.name, color: cat.color });
      } catch {
        setError('Failed to load category.');
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
        {isEdit ? 'Edit Category' : 'New Category'}
      </Typography.Title>

      <Formik<CategoryFormValues>
        initialValues={initialValues}
        validationSchema={categorySchema}
        enableReinitialize
        onSubmit={async (values, { setStatus }) => {
          try {
            if (isEdit) {
              await categoryApi.update(Number(id), values);
            } else {
              await categoryApi.create(values);
            }
            navigate('/admin/categories');
          } catch {
            setStatus('Failed to save category.');
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
                  onChange={(_, hex) => setFieldValue('color', hex)}
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
                <Button onClick={() => navigate('/admin/categories')}>Cancel</Button>
              </Space>
            </Form.Item>
          </Form>
        )}
      </Formik>
    </div>
  );
}

