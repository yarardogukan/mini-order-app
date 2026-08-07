import React from "react";
import ReactDOM from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router-dom";

import "./styles/global.css";

import App from "./App";
import ProductListPage from "./pages/products/ProductListPage";
import CreateOrderPage from "./pages/orders/CreateOrderPage";

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      {
        index: true,
        element: <ProductListPage />,
      },
      {
        path: "orders/create",
        element: <CreateOrderPage />,
      },
    ],
  },
]);

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
);
