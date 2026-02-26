import * as yup from 'yup';

export const loginSchema = yup.object({
  email: yup.string().email('Must be a valid email').required('Email is required'),
  password: yup.string().min(6, 'At least 6 characters').required('Password is required'),
});

export const registerSchema = yup.object({
  email: yup.string().email('Must be a valid email').required('Email is required'),
  password: yup.string().min(6, 'At least 6 characters').required('Password is required'),
});

export const expenseSchema = yup.object({
  amount: yup
    .number()
    .typeError('Amount must be a number')
    .positive('Amount must be positive')
    .required('Amount is required'),
  description: yup.string().trim().required('Description is required'),
  date: yup.string().required('Date is required'),
  categoryId: yup
    .number()
    .typeError('Category is required')
    .positive('Category is required')
    .required('Category is required'),
});

export const categorySchema = yup.object({
  name: yup.string().trim().required('Name is required'),
  color: yup
    .string()
    .matches(/^#[0-9A-Fa-f]{6}$/, 'Must be a valid hex color')
    .required('Color is required'),
});

export type LoginFormData = yup.InferType<typeof loginSchema>;
export type RegisterFormData = yup.InferType<typeof registerSchema>;
export type ExpenseFormData = yup.InferType<typeof expenseSchema>;
export type CategoryFormData = yup.InferType<typeof categorySchema>;
