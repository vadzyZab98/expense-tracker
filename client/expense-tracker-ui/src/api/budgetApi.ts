import api from './axiosInstance';
import type { MonthlyBudget } from '../types/models';

export interface BudgetPayload {
  categoryId: number;
  year: number;
  month: number;
  amount: number;
}

export const budgetApi = {
  getAll: () =>
    api.get<MonthlyBudget[]>('/api/budgets').then((r) => r.data),

  getByMonth: (year: number, month: number) =>
    api.get<MonthlyBudget[]>('/api/budgets', { params: { year, month } }).then((r) => r.data),

  create: (payload: BudgetPayload) =>
    api.post<MonthlyBudget>('/api/budgets', payload).then((r) => r.data),

  update: (id: number, payload: BudgetPayload) =>
    api.put(`/api/budgets/${id}`, payload),

  remove: (id: number) =>
    api.delete(`/api/budgets/${id}`),
};
