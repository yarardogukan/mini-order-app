import type { Brand } from "../types/brand";
import { API_BASE_URL } from "./api";

export async function getBrands(): Promise<Brand[]> {
  const response = await fetch(`${API_BASE_URL}/brands`);

  if (!response.ok) {
    throw new Error("Brands could not be loaded.");
  }

  return response.json();
}
