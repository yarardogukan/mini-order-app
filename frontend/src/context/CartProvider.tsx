import type { ReactNode } from "react";
import { useEffect, useState } from "react";

import {
  addCartItem,
  clearCart,
  getCart,
  removeCartItem,
  updateCartItemQuantity,
} from "../api/cartApi";

import type { Cart } from "../types/cart";
import {
  getStoredCartId,
  removeStoredCartId,
  storeCartId,
} from "../utils/cartStorage";
import { CartContext, type CartContextValue } from "./CartContext";

interface CartProviderProps {
  children: ReactNode;
}

export function CartProvider({ children }: CartProviderProps) {
  const [cart, setCart] = useState<Cart | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadCart = async () => {
      const cartId = getStoredCartId();

      if (!cartId) {
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError(null);

        const data = await getCart(cartId);

        setCart(data);
      } catch {
        removeStoredCartId();
        setCart(null);
        setError("Cart could not be restored.");
      } finally {
        setLoading(false);
      }
    };

    loadCart();
  }, []);

  const addItem = async (productId: number, quantity = 1) => {
    try {
      setError(null);

      const storedCartId = getStoredCartId();

      const updatedCart = await addCartItem(
        {
          productId,
          quantity,
        },
        storedCartId
      );

      storeCartId(updatedCart.cartId);
      setCart(updatedCart);
    } catch {
      setError("Product could not be added to cart.");

      throw new Error("Product could not be added to cart.");
    }
  };

  const updateItemQuantity = async (productId: number, quantity: number) => {
    const cartId = getStoredCartId();

    if (!cartId) {
      throw new Error("Cart could not be found.");
    }

    try {
      setError(null);

      const updatedCart = await updateCartItemQuantity(cartId, productId, {
        quantity,
      });

      setCart(updatedCart);
    } catch {
      setError("Cart item quantity could not be updated.");

      throw new Error("Cart item quantity could not be updated.");
    }
  };

  const removeItem = async (productId: number) => {
    const cartId = getStoredCartId();

    if (!cartId) {
      throw new Error("Cart could not be found.");
    }

    try {
      setError(null);

      const updatedCart = await removeCartItem(cartId, productId);

      setCart(updatedCart);
    } catch {
      setError("Cart item could not be removed.");

      throw new Error("Cart item could not be removed.");
    }
  };

  const clear = async () => {
    const cartId = getStoredCartId();

    if (!cartId) {
      return;
    }

    try {
      setError(null);

      await clearCart(cartId);

      setCart((current) =>
        current
          ? {
              ...current,
              itemCount: 0,
              subtotal: 0,
              total: 0,
              items: [],
            }
          : null
      );
    } catch {
      setError("Cart could not be cleared.");

      throw new Error("Cart could not be cleared.");
    }
  };

  const value: CartContextValue = {
    cart,
    loading,
    error,
    addItem,
    updateItemQuantity,
    removeItem,
    clear,
  };

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}
