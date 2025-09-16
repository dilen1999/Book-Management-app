import { API_BASE } from "@/config/apiConfig";

export type Book = {
  id: string;
  title: string;
  isbn: string;
  publishedYear: number;
};

export type CreateBookRequest = {
  title: string;
  isbn: string;
  publishedYear: number;
  authorIds: string[];
  categoryIds: string[];
};

async function apiFetch<T>(url: string, options?: RequestInit): Promise<T> {
  const token = sessionStorage.getItem("accessToken");

  const res = await fetch(`${API_BASE}${url}`, {
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options?.headers,
    },
    ...options,
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(`API error ${res.status}: ${text}`);
  }
  return res.json();
}

export const BooksApi = {
  getAll: (page = 1, pageSize = 20) =>
    apiFetch<Book[]>(`/Books?page=${page}&pageSize=${pageSize}`),

  getCount: () => apiFetch<{ totalCount: number }>(`/Books/count`),

  create: (data: CreateBookRequest) =>
    apiFetch<Book>(`/Books`, {
      method: "POST",
      body: JSON.stringify(data),
    }),
};
