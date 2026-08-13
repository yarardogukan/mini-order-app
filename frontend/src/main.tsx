import React from "react";
import ReactDOM from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import { CartProvider } from "./context/CartProvider";

import "./styles/global.css";

import App from "./App";
import CartPage from "./pages/cart/CartPage";
import CreateOrderPage from "./pages/orders/CreateOrderPage";
import OrderDetailPage from "./pages/orders/OrderDetailPage";
import OrderListPage from "./pages/orders/OrderListPage";
import ProductDetailPage from "./pages/products/ProductDetailPage";
import ProductListPage from "./pages/products/ProductListPage";
import WelcomePage from "./pages/WelcomePage";

const router = createBrowserRouter([
  {
    path: "/",
    element: <WelcomePage />,
  },
  {
    element: <App />,
    children: [
      {
        path: "products",
        element: <ProductListPage />,
      },
      {
        path: "products/:id",
        element: <ProductDetailPage />,
      },
      {
        path: "orders/create",
        element: <CreateOrderPage />,
      },
      {
        path: "orders",
        element: <OrderListPage />,
      },
      {
        path: "orders/:id",
        element: <OrderDetailPage />,
      },
      {
        path: "cart",
        element: <CartPage />,
      },
    ],
  },
]);

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <CartProvider>
      <RouterProvider router={router} />
    </CartProvider>
  </React.StrictMode>
);
