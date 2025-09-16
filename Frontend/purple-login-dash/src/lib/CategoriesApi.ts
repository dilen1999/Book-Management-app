import { API_BASE } from "@/config/apiConfig";

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

export const CategoriesApi = {
  getCount: () =>
    apiFetch<{ totalCount: number }>(`/Categories/count`), 
};
