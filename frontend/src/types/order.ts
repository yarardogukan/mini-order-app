export interface CreateOrderItemRequest {
  productId: number;
  quantity: number;
}

export interface CreateOrderRequest {
  customerName: string;
  items: CreateOrderItemRequest[];
}

export interface OrderItem {
  productId: number;
  productName: string;
  stockCode: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface Order {
  id: number;
  customerName: string;
  createdAt: string;
  totalAmount: number;
  items: OrderItem[];
}
