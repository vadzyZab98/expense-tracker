import api from './axiosInstance';
import type { Expense } from '../types/models';

export interface ExpensePayload {
  amount: number;
  description: string;
  date: string;
  categoryId: number;
}

export const expenseApi = {
  getAll: () =>
    api.get<Expense[]>('/api/expenses').then((r) => r.data),

  getById: (id: number) =>
    api.get<Expense>(`/api/expenses/${id}`).then((r) => r.data),

  create: (payload: ExpensePayload) =>
    api.post<Expense>('/api/expenses', payload).then((r) => r.data),

  update: (id: number, payload: ExpensePayload) =>
    api.put(`/api/expenses/${id}`, payload),

  remove: (id: number) =>
    api.delete(`/api/expenses/${id}`),
};
