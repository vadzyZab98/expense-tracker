import { useState, useEffect } from 'react';
import { useNavigate, useOutletContext } from 'react-router-dom';
import { Table, Button, Tag, Space, Alert, Modal, Typography, message } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, ExclamationCircleOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { incomeApi } from '../../api/incomeApi';
import { incomeCategoryApi } from '../../api/incomeCategoryApi';
import { extractErrorDetail } from '../../utils/errorUtils';
import type { Income, IncomeCategory } from '../../types/models';
import type { DashboardContext } from '../DashboardPage';

export default function IncomesTab() {
  const navigate = useNavigate();
  const { year, month, onDataChanged } = useOutletContext<DashboardContext>();
  const [incomes, setIncomes] = useState<Income[]>([]);
  const [incomeCategories, setIncomeCategories] = useState<IncomeCategory[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchData = async () => {
    setLoading(true);
    setError('');
    try {
      const [incomesData, categoriesData] = await Promise.all([
        incomeApi.getAll(),
        incomeCategoryApi.getAll(),
      ]);
      setIncomes(incomesData);
      setIncomeCategories(categoriesData);
    } catch {
      setError('Failed to load data.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchData(); }, []);

  const monthIncomes = incomes.filter((i) => {
    const d = new Date(i.date);
    return d.getFullYear() === year && d.getMonth() + 1 === month;
  });

  const totalIncome = monthIncomes.reduce((sum, i) => sum + i.amount, 0);

  const getCategoryById = (id: number) => incomeCategories.find((c) => c.id === id);

  const handleDelete = (id: number) => {
    Modal.confirm({
      title: 'Delete this income?',
      icon: <ExclamationCircleOutlined />,
      content: 'This may fail if budgets or expenses depend on this income.',
      okText: 'Delete',
      okType: 'danger',
      onOk: async () => {
        try {
          await incomeApi.remove(id);
          setIncomes((prev) => prev.filter((i) => i.id !== id));
          onDataChanged();
          message.success('Income deleted.');
        } catch (err) {
          message.error(extractErrorDetail(err));
        }
      },
    });
  };

  const columns: ColumnsType<Income> = [
    {
      title: 'Date',
      dataIndex: 'date',
      key: 'date',
      render: (date: string) => new Date(date).toLocaleDateString(),
    },
    {
      title: 'Category',
      dataIndex: 'incomeCategoryId',
      key: 'category',
      render: (_: number, record: Income) => {
        const cat = record.incomeCategory ?? getCategoryById(record.incomeCategoryId);
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
      render: (_: unknown, record: Income) => (
        <Space>
          <Button size="small" icon={<EditOutlined />} onClick={() => navigate(`/incomes/${record.id}/edit`)}>
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
      <Typography.Text strong style={{ fontSize: 16, display: 'block', marginBottom: 16 }}>
        Total Income for this month: ${totalIncome.toFixed(2)}
      </Typography.Text>

      <Space style={{ width: '100%', justifyContent: 'space-between', marginBottom: 16 }}>
        <Typography.Title level={5} style={{ margin: 0 }}>Income Records</Typography.Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => navigate('/incomes/new')}>
          New Income
        </Button>
      </Space>

      {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 16 }} />}

      <Table<Income>
        columns={columns}
        dataSource={monthIncomes}
        rowKey="id"
        loading={loading}
        pagination={false}
        locale={{ emptyText: 'No income records for this month.' }}
      />
    </div>
  );
}
