import { ValidationError } from 'yup';
import { expenseSchema } from '../ExpenseFormPage';

describe('expenseSchema', () => {
  const validExpense = {
    amount: 50.00,
    description: 'Lunch',
    date: '2026-02-26',
    categoryId: 1,
  };

  it('accepts valid expense data', async () => {
    const validInputs = [
      validExpense,
      { amount: 0.01, description: 'Minimum', date: '2025-01-01', categoryId: 5 },
      { amount: 9999.99, description: 'Large expense', date: '2026-12-31', categoryId: 100 },
    ];

    for (const input of validInputs) {
      await expect(expenseSchema.validate(input)).resolves.toBeTruthy();
    }
  });

  it('rejects amount equal to 0', async () => {
    await expect(expenseSchema.validate({ ...validExpense, amount: 0 }))
      .rejects.toBeInstanceOf(ValidationError);
  });

  it('rejects negative amount', async () => {
    const negativeAmounts = [-1, -0.01, -100];

    for (const amount of negativeAmounts) {
      await expect(expenseSchema.validate({ ...validExpense, amount }))
        .rejects.toBeInstanceOf(ValidationError);
    }
  });

  it('returns correct error message for amount below minimum', async () => {
    await expect(expenseSchema.validate({ ...validExpense, amount: 0 }))
      .rejects.toThrow('Amount must be greater than 0');
  });

  it('rejects missing description', async () => {
    await expect(expenseSchema.validate({ ...validExpense, description: '' }))
      .rejects.toBeInstanceOf(ValidationError);
  });

  it('returns correct error message for missing description', async () => {
    await expect(expenseSchema.validate({ ...validExpense, description: '' }))
      .rejects.toThrow('Description is required');
  });

  it('rejects missing date', async () => {
    await expect(expenseSchema.validate({ ...validExpense, date: '' }))
      .rejects.toBeInstanceOf(ValidationError);
  });

  it('returns correct error message for missing date', async () => {
    await expect(expenseSchema.validate({ ...validExpense, date: '' }))
      .rejects.toThrow('Date is required');
  });

  it('rejects categoryId of 0', async () => {
    await expect(expenseSchema.validate({ ...validExpense, categoryId: 0 }))
      .rejects.toBeInstanceOf(ValidationError);
  });

  it('rejects negative categoryId', async () => {
    const invalidIds = [-1, -100];

    for (const categoryId of invalidIds) {
      await expect(expenseSchema.validate({ ...validExpense, categoryId }))
        .rejects.toBeInstanceOf(ValidationError);
    }
  });

  it('returns correct error message for invalid categoryId', async () => {
    await expect(expenseSchema.validate({ ...validExpense, categoryId: 0 }))
      .rejects.toThrow('Category is required');
  });
});
