import { API_BASE_URL } from "./api";

import type { CreateOrderRequest, Order } from "../types/order";

export async function createOrder(request: CreateOrderRequest): Promise<Order> {
  const response = await fetch(`${API_BASE_URL}/orders`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  const data = await response.json();

  if (!response.ok) {
    throw new Error(data.message ?? "Order could not be created.");
  }

  return data;
}

export async function getOrders(): Promise<Order[]> {
  const response = await fetch(`${API_BASE_URL}/orders`);

  const data = await response.json();

  if (!response.ok) {
    throw new Error(data.message ?? "Orders could not be loaded.");
  }

  return data;
}

export async function getOrderById(id: number): Promise<Order> {
  const response = await fetch(`${API_BASE_URL}/orders/${id}`);

  const data = await response.json();

  if (!response.ok) {
    throw new Error(data.message ?? "Order could not be loaded.");
  }

  return data;
}
