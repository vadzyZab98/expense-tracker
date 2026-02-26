import type { AxiosError } from 'axios';
import api from './axiosInstance';

const fetcher = <T>(url: string): Promise<T> =>
  api.get<T>(url).then((res) => res.data);

export const getErrorMessage = (error: unknown): string => {
  const axiosError = error as AxiosError<{ message?: string }>;
  return axiosError.response?.data?.message ?? axiosError.message ?? 'Something went wrong.';
};

export default fetcher;
