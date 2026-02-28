import { Row, Col, Statistic, Card } from 'antd';
import { WarningOutlined } from '@ant-design/icons';

interface FinancialSummaryProps {
  totalIncome: number;
  totalUsed: number;
  usedLabel: string;
  remainingLabel: string;
}

export default function FinancialSummary({ totalIncome, totalUsed, usedLabel, remainingLabel }: FinancialSummaryProps) {
  const remaining = totalIncome - totalUsed;
  const isLimitReached = remaining <= 0 && totalIncome > 0;
  const hasNoIncome = totalIncome === 0;

  return (
    <Card
      size="small"
      style={{
        marginBottom: 16,
        border: isLimitReached ? '2px solid #ff4d4f' : undefined,
        background: isLimitReached ? '#fff2f0' : undefined,
      }}
    >
      <Row gutter={24}>
        <Col span={8}>
          <Statistic
            title="Total Income"
            value={totalIncome}
            precision={2}
            prefix="$"
            valueStyle={{ color: hasNoIncome ? '#faad14' : '#3f8600' }}
          />
        </Col>
        <Col span={8}>
          <Statistic
            title={usedLabel}
            value={totalUsed}
            precision={2}
            prefix="$"
          />
        </Col>
        <Col span={8}>
          <Statistic
            title={remainingLabel}
            value={remaining}
            precision={2}
            prefix={isLimitReached ? <WarningOutlined /> : '$'}
            suffix={isLimitReached ? undefined : undefined}
            valueStyle={{ color: isLimitReached ? '#ff4d4f' : remaining < totalIncome * 0.1 ? '#faad14' : '#3f8600' }}
          />
        </Col>
      </Row>
    </Card>
  );
}
