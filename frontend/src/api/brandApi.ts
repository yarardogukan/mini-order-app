import type {
  Brand,
  BrandDetail,
  CreateBrandRequest,
  UpdateBrandRequest,
} from "../types/brand";
import { API_BASE_URL } from "./api";

interface ApiErrorResponse {
  code?: string;
  message?: string;
}

export class BrandApiError extends Error {
  code: string;
  status: number;

  constructor(code: string, message: string, status: number) {
    super(message);

    this.name = "BrandApiError";
    this.code = code;
    this.status = status;
  }
}

async function throwBrandApiError(
  response: Response,
  fallbackMessage: string
): Promise<never> {
  let errorBody: ApiErrorResponse | null = null;

  try {
    errorBody = await response.json();
  } catch {
    // Response body may be empty or not JSON.
  }

  throw new BrandApiError(
    errorBody?.code ?? "Brand.UnknownError",
    errorBody?.message ?? fallbackMessage,
    response.status
  );
}

export async function getBrands(): Promise<Brand[]> {
  const response = await fetch(`${API_BASE_URL}/brands`);

  if (!response.ok) {
    return throwBrandApiError(response, "Brands could not be loaded.");
  }

  return response.json();
}

export async function getBrandById(id: number): Promise<BrandDetail> {
  const response = await fetch(`${API_BASE_URL}/brands/${id}`);

  if (!response.ok) {
    return throwBrandApiError(response, "Brand could not be loaded.");
  }

  return response.json();
}

export async function createBrand(
  request: CreateBrandRequest
): Promise<BrandDetail> {
  const response = await fetch(`${API_BASE_URL}/brands`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    return throwBrandApiError(response, "Brand could not be created.");
  }

  return response.json();
}

export async function updateBrand(
  id: number,
  request: UpdateBrandRequest
): Promise<BrandDetail> {
  const response = await fetch(`${API_BASE_URL}/brands/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    return throwBrandApiError(response, "Brand could not be updated.");
  }

  return response.json();
}

export async function deleteBrand(id: number): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/brands/${id}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    return throwBrandApiError(response, "Brand could not be deleted.");
  }
}
