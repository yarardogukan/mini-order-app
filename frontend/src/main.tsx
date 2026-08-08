import React from "react";
import ReactDOM from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router-dom";

import "./styles/global.css";

import App from "./App";
import ProductListPage from "./pages/products/ProductListPage";
import CreateOrderPage from "./pages/orders/CreateOrderPage";
import OrderListPage from "./pages/orders/OrderListPage";
import OrderDetailPage from "./pages/orders/OrderDetailPage";
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
    ],
  },
]);

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
);
