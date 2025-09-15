import { API_BASE } from "@/config/apiConfig";

export type AuthResponse = {
  userId: string;
  email: string;
  name: string;
  role: string;
  accessToken: string;
  refreshToken: string;
};

async function apiFetch<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${API_BASE}${url}`, {
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
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

export const AuthApi = {
  register: (email: string, name: string, password: string, role: string) =>
    apiFetch<AuthResponse>("/Auth/register", {
      method: "POST",
      body: JSON.stringify({ email, name, password, role }),
    }),

  login: (email: string, password: string) =>
    apiFetch<AuthResponse>("/Auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    }),
};
