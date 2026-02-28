import * as Yup from 'yup';

export const budgetSchema = Yup.object({
  categoryId: Yup.number().min(1, 'Category is required').required('Category is required'),
  year: Yup.number().min(2000, 'Year must be between 2000 and 2100').max(2100, 'Year must be between 2000 and 2100').required('Year is required'),
  month: Yup.number().min(1, 'Month must be between 1 and 12').max(12, 'Month must be between 1 and 12').required('Month is required'),
  amount: Yup.number().min(0.01, 'Amount must be greater than 0').required('Amount is required'),
});
