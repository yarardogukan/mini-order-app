import { createContext } from "react";
import type { Cart } from "../types/cart";

export interface CartContextValue {
  cart: Cart | null;
  loading: boolean;
  error: string | null;

  addItem: (productId: number, quantity?: number) => Promise<void>;

  updateItemQuantity: (productId: number, quantity: number) => Promise<void>;

  removeItem: (productId: number) => Promise<void>;

  clear: () => Promise<void>;
}

export const CartContext = createContext<CartContextValue | undefined>(
  undefined
);
