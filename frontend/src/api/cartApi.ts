import type {
  AddCartItemRequest,
  Cart,
  UpdateCartItemRequest,
} from "../types/cart";
import { API_BASE_URL } from "./api";

interface ApiErrorResponse {
  code?: string;
  message?: string;
}

export class CartApiError extends Error {
  code: string;
  status: number;

  constructor(code: string, message: string, status: number) {
    super(message);

    this.name = "CartApiError";
    this.code = code;
    this.status = status;
  }
}

async function throwCartApiError(
  response: Response,
  fallbackMessage: string
): Promise<never> {
  let errorBody: ApiErrorResponse | null = null;

  try {
    errorBody = await response.json();
  } catch {
    // Response body may be empty or not JSON.
  }

  throw new CartApiError(
    errorBody?.code ?? "Cart.UnknownError",
    errorBody?.message ?? fallbackMessage,
    response.status
  );
}

export async function getCart(cartId: string): Promise<Cart> {
  const response = await fetch(`${API_BASE_URL}/cart/${cartId}`);

  if (!response.ok) {
    return throwCartApiError(response, "Cart could not be loaded.");
  }

  return response.json();
}

export async function addCartItem(
  request: AddCartItemRequest,
  cartId?: string | null
): Promise<Cart> {
  const query = cartId ? `?cartId=${encodeURIComponent(cartId)}` : "";

  const response = await fetch(`${API_BASE_URL}/cart/items${query}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    return throwCartApiError(response, "Product could not be added to cart.");
  }

  return response.json();
}

export async function updateCartItemQuantity(
  cartId: string,
  productId: number,
  request: UpdateCartItemRequest
): Promise<Cart> {
  const response = await fetch(
    `${API_BASE_URL}/cart/${cartId}/items/${productId}`,
    {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(request),
    }
  );

  if (!response.ok) {
    return throwCartApiError(
      response,
      "Cart item quantity could not be updated."
    );
  }

  return response.json();
}

export async function removeCartItem(
  cartId: string,
  productId: number
): Promise<Cart> {
  const response = await fetch(
    `${API_BASE_URL}/cart/${cartId}/items/${productId}`,
    {
      method: "DELETE",
    }
  );

  if (!response.ok) {
    return throwCartApiError(response, "Cart item could not be removed.");
  }

  return response.json();
}

export async function clearCart(cartId: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/cart/${cartId}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    return throwCartApiError(response, "Cart could not be cleared.");
  }
}
