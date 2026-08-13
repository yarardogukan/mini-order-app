export interface CartItem {
  productId: number;
  stockCode: string;
  productName: string;
  brandName: string;
  categoryName: string;
  coverImageUrl: string | null;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface Cart {
  cartId: string;
  itemCount: number;
  subtotal: number;
  total: number;
  items: CartItem[];
}

export interface AddCartItemRequest {
  productId: number;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}
