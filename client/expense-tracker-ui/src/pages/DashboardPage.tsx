import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Table, Button, Tag, Select, Space, Alert, Modal, Typography, message } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, ExclamationCircleOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { expenseApi } from '../api/expenseApi';
import { categoryApi } from '../api/categoryApi';
import type { Expense, Category } from '../types/models';

export default function DashboardPage() {
  const navigate = useNavigate();
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [filterCategoryId, setFilterCategoryId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchData = async () => {
    setLoading(true);
    setError('');
    try {
      const [expensesData, categoriesData] = await Promise.all([
        expenseApi.getAll(),
        categoryApi.getAll(),
      ]);
      setExpenses(expensesData);
      setCategories(categoriesData);
    } catch {
      setError('Failed to load data.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchData(); }, []);

  const handleDelete = (id: number) => {
    Modal.confirm({
      title: 'Delete this expense?',
      icon: <ExclamationCircleOutlined />,
      okText: 'Delete',
      okType: 'danger',
      onOk: async () => {
        try {
          await expenseApi.remove(id);
          setExpenses((prev) => prev.filter((e) => e.id !== id));
          message.success('Expense deleted.');
        } catch {
          message.error('Failed to delete expense.');
        }
      },
    });
  };

  const getCategoryById = (id: number) => categories.find((c) => c.id === id);

  const filtered = filterCategoryId
    ? expenses.filter((e) => e.categoryId === filterCategoryId)
    : expenses;

  const total = filtered.reduce((sum, e) => sum + e.amount, 0);

  const columns: ColumnsType<Expense> = [
    {
      title: 'Date',
      dataIndex: 'date',
      key: 'date',
      render: (date: string) => new Date(date).toLocaleDateString(),
    },
    {
      title: 'Description',
      dataIndex: 'description',
      key: 'description',
    },
    {
      title: 'Category',
      dataIndex: 'categoryId',
      key: 'category',
      render: (_: number, record: Expense) => {
        const cat = record.category ?? getCategoryById(record.categoryId);
        return cat ? <Tag color={cat.color}>{cat.name}</Tag> : '—';
      },
    },
    {
      title: 'Amount',
      dataIndex: 'amount',
      key: 'amount',
      align: 'right',
      render: (amount: number) => `$${amount.toFixed(2)}`,
    },
    {
      title: 'Actions',
      key: 'actions',
      align: 'center',
      render: (_: unknown, record: Expense) => (
        <Space>
          <Button size="small" icon={<EditOutlined />} onClick={() => navigate(`/expenses/${record.id}/edit`)}>
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
        <Typography.Title level={4} style={{ margin: 0 }}>My Expenses</Typography.Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => navigate('/expenses/new')}>
          New Expense
        </Button>
      </Space>

      <Space style={{ marginBottom: 16 }}>
        <Typography.Text strong>Filter by category:</Typography.Text>
        <Select
          allowClear
          placeholder="All"
          value={filterCategoryId}
          onChange={(val) => setFilterCategoryId(val ?? null)}
          style={{ width: 200 }}
          options={categories.map((c) => ({ label: c.name, value: c.id }))}
        />
        <Typography.Text strong style={{ fontSize: 16 }}>
          Total: ${total.toFixed(2)}
        </Typography.Text>
      </Space>

      {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 16 }} />}

      <Table<Expense>
        columns={columns}
        dataSource={filtered}
        rowKey="id"
        loading={loading}
        pagination={false}
        locale={{ emptyText: 'No expenses found.' }}
      />
    </div>
  );
}
