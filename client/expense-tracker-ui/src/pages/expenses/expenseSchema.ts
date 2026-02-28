import * as Yup from 'yup';

export const expenseSchema = Yup.object({
  amount: Yup.number().min(0.01, 'Amount must be greater than 0').required('Amount is required'),
  description: Yup.string().required('Description is required'),
  date: Yup.string().required('Date is required'),
  categoryId: Yup.number().min(1, 'Category is required').required('Category is required'),
});
