const CART_ID_KEY = "mini-order-cart-id";

export function getStoredCartId(): string | null {
  return localStorage.getItem(CART_ID_KEY);
}

export function storeCartId(cartId: string): void {
  localStorage.setItem(CART_ID_KEY, cartId);
}

export function removeStoredCartId(): void {
  localStorage.removeItem(CART_ID_KEY);
}
