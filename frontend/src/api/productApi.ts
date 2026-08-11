import type { Product, ProductDetail } from "../types/product";
import { API_BASE_URL } from "./api";

interface GetProductsParams {
  search?: string;
  categoryId?: number | null;
}

export async function getProducts(
  params: GetProductsParams = {}
): Promise<Product[]> {
  const queryParams = new URLSearchParams();

  if (params.search?.trim()) {
    queryParams.set("search", params.search.trim());
  }

  if (params.categoryId != null) {
    queryParams.set("categoryId", params.categoryId.toString());
  }

  const queryString = queryParams.toString();

  const url = queryString
    ? `${API_BASE_URL}/products?${queryString}`
    : `${API_BASE_URL}/products`;

  const response = await fetch(url);

  if (!response.ok) {
    throw new Error("Products could not be loaded.");
  }

  return response.json();
}

export async function getProductById(id: number): Promise<ProductDetail> {
  const response = await fetch(`${API_BASE_URL}/products/${id}`);

  if (!response.ok) {
    throw new Error("Product could not be loaded.");
  }

  return response.json();
}
