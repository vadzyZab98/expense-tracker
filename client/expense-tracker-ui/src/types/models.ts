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

export interface TokenResponse {
  token: string;
}
