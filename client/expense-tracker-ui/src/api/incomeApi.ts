import api from './axiosInstance';
import type { Income } from '../types/models';

export interface IncomePayload {
  amount: number;
  date: string;
  incomeCategoryId: number;
}

export const incomeApi = {
  getAll: () =>
    api.get<Income[]>('/api/incomes').then((r) => r.data),

  getById: (id: number) =>
    api.get<Income>(`/api/incomes/${id}`).then((r) => r.data),

  create: (payload: IncomePayload) =>
    api.post<Income>('/api/incomes', payload).then((r) => r.data),

  update: (id: number, payload: IncomePayload) =>
    api.put(`/api/incomes/${id}`, payload),

  remove: (id: number) =>
    api.delete(`/api/incomes/${id}`),
};
