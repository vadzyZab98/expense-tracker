import api from './axiosInstance';
import type { TokenResponse } from '../types/models';

export const authApi = {
  login: (email: string, password: string) =>
    api.post<TokenResponse>('/api/auth/login', { email, password }).then((r) => r.data),

  register: (email: string, password: string) =>
    api.post<TokenResponse>('/api/auth/register', { email, password }).then((r) => r.data),
};
