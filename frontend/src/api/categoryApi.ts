import type { Category } from "../types/category";
import { API_BASE_URL } from "./api";

export async function getCategories(): Promise<Category[]> {
  const response = await fetch(`${API_BASE_URL}/categories`);

  const data = await response.json();

  if (!response.ok) {
    throw new Error(data.message ?? "Categories could not be loaded.");
  }

  return data;
}
