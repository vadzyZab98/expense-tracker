import api from './axiosInstance';
import type { Category } from '../types/models';

export interface CategoryPayload {
  name: string;
  color: string;
}

export const categoryApi = {
  getAll: () =>
    api.get<Category[]>('/api/categories').then((r) => r.data),

  getById: (id: number) =>
    api.get<Category>(`/api/categories/${id}`).then((r) => r.data),

  create: (payload: CategoryPayload) =>
    api.post<Category>('/api/categories', payload).then((r) => r.data),

  update: (id: number, payload: CategoryPayload) =>
    api.put(`/api/categories/${id}`, payload),

  remove: (id: number) =>
    api.delete(`/api/categories/${id}`),
};
