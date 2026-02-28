export interface Category {
  id: number;
  name: string;
  color: string;
}

export interface Expense {
  id: number;
  userId: number;
  categoryId: number;
  category?: Category;
  amount: number;
  description: string;
  date: string;
}

export interface User {
  id: number;
  email: string;
  role: 'User' | 'Admin' | 'SuperAdmin';
}

export interface IncomeCategory {
  id: number;
  name: string;
  color: string;
}

export interface Income {
  id: number;
  userId: number;
  amount: number;
  date: string;
  incomeCategoryId: number;
  incomeCategory?: IncomeCategory;
}

export interface MonthlyBudget {
  id: number;
  categoryId: number;
  category?: Category;
  year: number;
  month: number;
  amount: number;
}

export interface TokenResponse {
  token: string;
}
