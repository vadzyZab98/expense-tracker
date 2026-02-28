import { useState, useEffect } from 'react';
import { useNavigate, useOutletContext } from 'react-router-dom';
import { Table, Button, Tag, Space, Alert, Modal, Typography, Progress, Tooltip, message } from 'antd';
import { PlusOutlined, DeleteOutlined, EditOutlined, ExclamationCircleOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { budgetApi } from '../../api/budgetApi';
import { expenseApi } from '../../api/expenseApi';
import { extractErrorDetail } from '../../utils/errorUtils';
import FinancialSummary from '../../components/FinancialSummary';
import type { MonthlyBudget, Expense } from '../../types/models';
import type { DashboardContext } from '../DashboardPage';

interface BudgetRow extends MonthlyBudget {
  spent: number;
  percentUsed: number;
}

export default function BudgetsTab() {
  const navigate = useNavigate();
  const { year, month, totalIncome, onDataChanged } = useOutletContext<DashboardContext>();
  const [budgets, setBudgets] = useState<MonthlyBudget[]>([]);
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchData = async () => {
    setLoading(true);
    setError('');
    try {
      const [budgetsData, expensesData] = await Promise.all([
        budgetApi.getByMonth(year, month),
        expenseApi.getAll(),
      ]);
      setBudgets(budgetsData);
      setExpenses(expensesData);
    } catch {
      setError('Failed to load data.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchData(); }, [year, month]);

  const monthExpenses = expenses.filter((e) => {
    const d = new Date(e.date);
    return d.getFullYear() === year && d.getMonth() + 1 === month;
  });

  const rows: BudgetRow[] = budgets.map((b) => {
    const spent = monthExpenses
      .filter((e) => e.categoryId === b.categoryId)
      .reduce((sum, e) => sum + e.amount, 0);
    const percentUsed = b.amount > 0 ? Math.round((spent / b.amount) * 100) : 0;
    return { ...b, spent, percentUsed };
  });

  const totalBudgeted = budgets.reduce((sum, b) => sum + b.amount, 0);
  const remainingToAllocate = totalIncome - totalBudgeted;
  const canCreate = totalIncome > 0 && remainingToAllocate > 0;

  const handleDelete = (id: number) => {
    Modal.confirm({
      title: 'Delete this budget?',
      icon: <ExclamationCircleOutlined />,
      okText: 'Delete',
      okType: 'danger',
      onOk: async () => {
        try {
          await budgetApi.remove(id);
          setBudgets((prev) => prev.filter((b) => b.id !== id));
          onDataChanged();
          message.success('Budget deleted.');
        } catch (err) {
          message.error(extractErrorDetail(err));
        }
      },
    });
  };

  const columns: ColumnsType<BudgetRow> = [
    {
      title: 'Category',
      dataIndex: 'categoryId',
      key: 'category',
      render: (_: number, record: BudgetRow) =>
        record.category ? <Tag color={record.category.color}>{record.category.name}</Tag> : '—',
    },
    {
      title: 'Budget',
      dataIndex: 'amount',
      key: 'amount',
      align: 'right',
      render: (amount: number) => `$${amount.toFixed(2)}`,
    },
    {
      title: 'Spent',
      dataIndex: 'spent',
      key: 'spent',
      align: 'right',
      render: (spent: number) => `$${spent.toFixed(2)}`,
    },
    {
      title: '% Used',
      dataIndex: 'percentUsed',
      key: 'percentUsed',
      align: 'center',
      width: 180,
      render: (pct: number) => (
        <Progress
          percent={pct}
          size="small"
          status={pct >= 100 ? 'exception' : pct >= 80 ? 'active' : 'normal'}
        />
      ),
    },
    {
      title: 'Actions',
      key: 'actions',
      align: 'center',
      render: (_: unknown, record: BudgetRow) => (
        <Space>
          <Button size="small" icon={<EditOutlined />} onClick={() => navigate(`/budgets/${record.id}/edit`)}>
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
        totalUsed={totalBudgeted}
        usedLabel="Total Planned Budgets"
        remainingLabel="Remaining To Allocate"
      />

      <Space style={{ width: '100%', justifyContent: 'space-between', marginBottom: 16 }}>
        <Typography.Title level={5} style={{ margin: 0 }}>Monthly Budgets</Typography.Title>
        <Tooltip title={!canCreate ? 'No remaining income to allocate for this month' : undefined}>
          <Button type="primary" icon={<PlusOutlined />} disabled={!canCreate} onClick={() => navigate(`/budgets/new?year=${year}&month=${month}`)}>
            New Budget
          </Button>
        </Tooltip>
      </Space>

      {error && <Alert message={error} type="error" showIcon style={{ marginBottom: 16 }} />}

      <Table<BudgetRow>
        columns={columns}
        dataSource={rows}
        rowKey="id"
        loading={loading}
        pagination={false}
        locale={{ emptyText: 'No budgets for this month.' }}
      />
    </div>
  );
}
