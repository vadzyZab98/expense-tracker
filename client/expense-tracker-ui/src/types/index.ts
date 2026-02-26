export interface User {
  id: number;
  email: string;
  role: 'User' | 'Admin';
}

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

export interface LoginPayload {
  email: string;
  password: string;
}

export interface RegisterPayload {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
}

export interface ExpensePayload {
  amount: number;
  description: string;
  date: string;
  categoryId: number;
}

export interface CategoryPayload {
  name: string;
  color: string;
}

export interface JwtPayload {
  sub: string;
  email: string;
  role?: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string;
  exp: number;
}
