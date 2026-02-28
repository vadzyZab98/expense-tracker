import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Table, Button, Tag, Space, Alert, Modal, Typography, message } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, ExclamationCircleOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { incomeCategoryApi } from '../../../api/incomeCategoryApi';
import { extractErrorDetail } from '../../../utils/errorUtils';
import type { IncomeCategory } from '../../../types/models';

export default function IncomeCategoriesPage() {
  const navigate = useNavigate();
  const [categories, setCategories] = useState<IncomeCategory[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchCategories = async () => {
    setLoading(true);
    setError('');
    try {
      setCategories(await incomeCategoryApi.getAll());
    } catch {
      setError('Failed to load income categories.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchCategories(); }, []);

  const handleDelete = (id: number) => {
    Modal.confirm({
      title: 'Delete this income category?',
      icon: <ExclamationCircleOutlined />,
      content: 'This may fail if incomes reference this category.',
      okText: 'Delete',
      okType: 'danger',
      onOk: async () => {
        try {
          await incomeCategoryApi.remove(id);
          setCategories((prev) => prev.filter((c) => c.id !== id));
          message.success('Income category deleted.');
        } catch (err) {
          message.error(extractErrorDetail(err));
        }
      },
    });
  };

  const columns: ColumnsType<IncomeCategory> = [
    {
      title: 'Color',
      dataIndex: 'color',
      key: 'color',
      width: 80,
      render: (color: string) => <Tag color={color}>{'\u00A0\u00A0\u00A0'}</Tag>,
    },
    {
      title: 'Name',
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: 'Actions',
      key: 'actions',
      align: 'center',
      width: 200,
      render: (_: unknown, record: IncomeCategory) => (
        <Space>
          <Button size="small" icon={<EditOutlined />} onClick={() => navigate(`/admin/income-categories/${record.id}/edit`)}>
            Edit
          </Button>
          <Button size="small" danger icon={<DeleteOutlined />} onClick={() => handleDelete(record.id)}>
            Delete
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <Space style={{ width: '100%', justifyContent: 'space-between', marginBottom: 16 }}>
        <Typography.Title level={4} style={{ margin: 0 }}>Income Categories</Typography.Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => navigate('/admin/income-categories/new')}>
          New Income Category
        </Button>
      </Space>

      {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 16 }} />}

      <Table<IncomeCategory>
        columns={columns}
        dataSource={categories}
        rowKey="id"
        loading={loading}
        pagination={false}
        locale={{ emptyText: 'No income categories yet.' }}
      />
    </div>
  );
}
