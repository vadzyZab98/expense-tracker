import api from './axiosInstance';
import type { IncomeCategory } from '../types/models';

export interface IncomeCategoryPayload {
  name: string;
  color: string;
}

export const incomeCategoryApi = {
  getAll: () =>
    api.get<IncomeCategory[]>('/api/income-categories').then((r) => r.data),

  getById: (id: number) =>
    api.get<IncomeCategory>(`/api/income-categories/${id}`).then((r) => r.data),

  create: (payload: IncomeCategoryPayload) =>
    api.post<IncomeCategory>('/api/income-categories', payload).then((r) => r.data),

  update: (id: number, payload: IncomeCategoryPayload) =>
    api.put(`/api/income-categories/${id}`, payload),

  remove: (id: number) =>
    api.delete(`/api/income-categories/${id}`),
};
