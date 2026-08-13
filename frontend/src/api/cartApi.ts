import type {
  AddCartItemRequest,
  Cart,
  UpdateCartItemRequest,
} from "../types/cart";
import { API_BASE_URL } from "./api";

export async function getCart(cartId: string): Promise<Cart> {
  const response = await fetch(`${API_BASE_URL}/cart/${cartId}`);

  if (!response.ok) {
    throw new Error("Cart could not be loaded.");
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
    throw new Error("Product could not be added to cart.");
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
    throw new Error("Cart item quantity could not be updated.");
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
    throw new Error("Cart item could not be removed.");
  }

  return response.json();
}

export async function clearCart(cartId: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/cart/${cartId}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    throw new Error("Cart could not be cleared.");
  }
}
