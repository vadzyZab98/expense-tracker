import { useState, useCallback } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { Tabs, DatePicker, Typography, Space } from 'antd';
import { DollarOutlined, WalletOutlined, FundOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { incomeApi } from '../api/incomeApi';
import { useEffect } from 'react';
import type { Income } from '../types/models';

export interface DashboardContext {
  year: number;
  month: number;
  totalIncome: number;
  onDataChanged: () => void;
}

const tabRoutes = [
  { key: '/', label: 'Expenses', icon: <DollarOutlined /> },
  { key: '/incomes', label: 'Incomes', icon: <WalletOutlined /> },
  { key: '/budgets', label: 'Budgets', icon: <FundOutlined /> },
];

export default function DashboardPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const now = dayjs();
  const [selectedMonth, setSelectedMonth] = useState(now);
  const [incomes, setIncomes] = useState<Income[]>([]);

  const year = selectedMonth.year();
  const month = selectedMonth.month() + 1;

  const fetchIncomes = useCallback(async () => {
    try {
      const data = await incomeApi.getAll();
      setIncomes(data);
    } catch {
      // silently fail — tabs will show their own errors
    }
  }, []);

  useEffect(() => { fetchIncomes(); }, [fetchIncomes]);

  const totalIncome = incomes
    .filter((i) => {
      const d = new Date(i.date);
      return d.getFullYear() === year && d.getMonth() + 1 === month;
    })
    .reduce((sum, i) => sum + i.amount, 0);

  const activeKey = tabRoutes.find((t) => t.key === location.pathname)?.key ?? '/';

  const handleTabChange = (key: string) => {
    navigate(key);
  };

  const handleMonthChange = (value: dayjs.Dayjs | null) => {
    if (value) {
      setSelectedMonth(value);
    }
  };

  const onDataChanged = useCallback(() => {
    fetchIncomes();
  }, [fetchIncomes]);

  const context: DashboardContext = { year, month, totalIncome, onDataChanged };

  return (
    <div style={{ padding: 24 }}>
      <Space style={{ width: '100%', justifyContent: 'space-between', marginBottom: 16 }}>
        <Typography.Title level={4} style={{ margin: 0 }}>Dashboard</Typography.Title>
        <Space>
          <Typography.Text strong>Month:</Typography.Text>
          <DatePicker
            picker="month"
            value={selectedMonth}
            onChange={handleMonthChange}
            allowClear={false}
          />
        </Space>
      </Space>

      <Tabs
        activeKey={activeKey}
        onChange={handleTabChange}
        items={tabRoutes.map((t) => ({
          key: t.key,
          label: (
            <span>
              {t.icon} {t.label}
            </span>
          ),
        }))}
      />

      <Outlet context={context} />
    </div>
  );
}
