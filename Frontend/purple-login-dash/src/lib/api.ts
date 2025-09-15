import { API_BASE } from "@/config/apiConfig";

export type Book = {
  id: string;
  title: string;
  isbn: string;
  publishedYear: number;
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

// BOOKS API
export const BooksApi = {
  getAll: (page = 1, pageSize = 20) =>
    apiFetch<Book[]>(`/Books?page=${page}&pageSize=${pageSize}`),
};
