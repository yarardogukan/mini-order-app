import type {
  Category,
  CategoryDetail,
  CreateCategoryRequest,
  UpdateCategoryRequest,
} from "../types/category";
import { API_BASE_URL } from "./api";

interface ApiErrorResponse {
  code?: string;
  message?: string;
}

export class CategoryApiError extends Error {
  code: string;
  status: number;

  constructor(code: string, message: string, status: number) {
    super(message);

    this.name = "CategoryApiError";
    this.code = code;
    this.status = status;
  }
}

async function throwCategoryApiError(
  response: Response,
  fallbackMessage: string
): Promise<never> {
  let errorBody: ApiErrorResponse | null = null;

  try {
    errorBody = await response.json();
  } catch {
    // Response body may be empty or not JSON.
  }

  throw new CategoryApiError(
    errorBody?.code ?? "Category.UnknownError",
    errorBody?.message ?? fallbackMessage,
    response.status
  );
}

export async function getCategories(): Promise<Category[]> {
  const response = await fetch(`${API_BASE_URL}/categories`);

  const data = await response.json();

  if (!response.ok) {
    return throwCategoryApiError(response, "Categories could not be loaded.");
  }

  return data;
}

export async function getCategoryById(id: number): Promise<CategoryDetail> {
  const response = await fetch(`${API_BASE_URL}/categories/${id}`);

  const data = await response.json();

  if (!response.ok) {
    return throwCategoryApiError(response, "Category could not be loaded.");
  }

  return data;
}

export async function createCategory(
  request: CreateCategoryRequest
): Promise<CategoryDetail> {
  const response = await fetch(`${API_BASE_URL}/categories`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  const data = await response.json();

  if (!response.ok) {
    return throwCategoryApiError(response, "Category could not be created.");
  }

  return data;
}

export async function updateCategory(
  id: number,
  request: UpdateCategoryRequest
): Promise<CategoryDetail> {
  const response = await fetch(`${API_BASE_URL}/categories/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  const data = await response.json();

  if (!response.ok) {
    return throwCategoryApiError(response, "Category could not be updated.");
  }

  return data;
}

export async function deleteCategory(id: number): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/categories/${id}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    return throwCategoryApiError(response, "Category could not be deleted.");
  }
}
