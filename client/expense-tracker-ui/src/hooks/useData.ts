import useSWR from 'swr';
import type { Expense, Category } from '@/types';
import fetcher from '@/api/fetcher';

export const useExpenses = () => {
  const { data, error, isLoading, mutate } = useSWR<Expense[]>('/api/expenses', fetcher);

  return {
    expenses: data ?? [],
    isLoading,
    isError: !!error,
    error,
    mutate,
  };
};

export const useExpense = (id: string | undefined) => {
  const { data, error, isLoading } = useSWR<Expense>(
    id ? `/api/expenses/${id}` : null,
    fetcher,
  );

  return {
    expense: data,
    isLoading,
    isError: !!error,
  };
};

export const useCategories = () => {
  const { data, error, isLoading, mutate } = useSWR<Category[]>('/api/categories', fetcher);

  return {
    categories: data ?? [],
    isLoading,
    isError: !!error,
    error,
    mutate,
  };
};

export const useCategory = (id: string | undefined) => {
  const { data, error, isLoading } = useSWR<Category>(
    id ? `/api/categories/${id}` : null,
    fetcher,
  );

  return {
    category: data,
    isLoading,
    isError: !!error,
  };
};
