import { useState, useEffect } from 'react';
import { useNavigate, useOutletContext } from 'react-router-dom';
import { Table, Button, Tag, Select, Space, Alert, Modal, Typography, Tooltip, message } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, ExclamationCircleOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { expenseApi } from '../../api/expenseApi';
import { categoryApi } from '../../api/categoryApi';
import { extractErrorDetail } from '../../utils/errorUtils';
import FinancialSummary from '../../components/FinancialSummary';
import type { Expense, Category } from '../../types/models';
import type { DashboardContext } from '../DashboardPage';

export default function ExpensesTab() {
  const navigate = useNavigate();
  const { year, month, totalIncome, onDataChanged } = useOutletContext<DashboardContext>();
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

  const monthExpenses = expenses.filter((e) => {
    const d = new Date(e.date);
    return d.getFullYear() === year && d.getMonth() + 1 === month;
  });

  const filtered = filterCategoryId
    ? monthExpenses.filter((e) => e.categoryId === filterCategoryId)
    : monthExpenses;

  const totalSpent = monthExpenses.reduce((sum, e) => sum + e.amount, 0);
  const filteredTotal = filtered.reduce((sum, e) => sum + e.amount, 0);
  const remaining = totalIncome - totalSpent;
  const canCreate = totalIncome > 0 && remaining > 0;

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
          onDataChanged();
          message.success('Expense deleted.');
        } catch (err) {
          message.error(extractErrorDetail(err));
        }
      },
    });
  };

  const getCategoryById = (id: number) => categories.find((c) => c.id === id);

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
    <div>
      <FinancialSummary
        totalIncome={totalIncome}
        totalUsed={totalSpent}
        usedLabel="Total Spent"
        remainingLabel="Remaining Available"
      />

      <Space style={{ width: '100%', justifyContent: 'space-between', marginBottom: 16 }}>
        <Space>
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
            Showing: ${filteredTotal.toFixed(2)}
          </Typography.Text>
        </Space>
        <Tooltip title={!canCreate ? 'No remaining income available for this month' : undefined}>
          <Button type="primary" icon={<PlusOutlined />} disabled={!canCreate} onClick={() => navigate('/expenses/new')}>
            New Expense
          </Button>
        </Tooltip>
      </Space>

      {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 16 }} />}

      <Table<Expense>
        columns={columns}
        dataSource={filtered}
        rowKey="id"
        loading={loading}
        pagination={false}
        locale={{ emptyText: 'No expenses for this month.' }}
      />
    </div>
  );
}
