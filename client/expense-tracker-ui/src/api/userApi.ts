import api from './axiosInstance';
import type { User } from '../types/models';

export const userApi = {
  getAll: () =>
    api.get<User[]>('/api/users').then((r) => r.data),

  updateRole: (id: number, role: string) =>
    api.put(`/api/users/${id}/role`, { role }),
};
