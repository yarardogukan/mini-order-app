import { useEffect, useMemo, useState } from "react";
import { API_BASE_URL } from "../../api/api";
import type { Product } from "../../types/product";
import type { CreateOrderRequest } from "../../types/order";
import { createOrder } from "../../api/orderApi";

function CreateOrderPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [customerName, setCustomerName] = useState("");
  const [quantities, setQuantities] = useState<Record<number, number>>({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        setLoading(true);
        setError(null);

        const response = await fetch(`${API_BASE_URL}/products`);

        if (!response.ok) {
          throw new Error("Products could not be loaded.");
        }

        const data: Product[] = await response.json();

        setProducts(data);
      } catch {
        setError("Products could not be loaded.");
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);

  const updateQuantity = (productId: number, quantity: number) => {
    setQuantities((current) => ({
      ...current,
      [productId]: Math.max(0, quantity),
    }));
  };

  const totalAmount = useMemo(() => {
    return products.reduce((total, product) => {
      const quantity = quantities[product.id] ?? 0;

      return total + product.price * quantity;
    }, 0);
  }, [products, quantities]);

  const handleSubmit = async () => {
    setSubmitError(null);
    setSuccessMessage(null);

    if (!customerName.trim()) {
      setSubmitError("Customer name is required.");
      return;
    }

    const items = Object.entries(quantities)
      .filter(([, quantity]) => quantity > 0)
      .map(([productId, quantity]) => ({
        productId: Number(productId),
        quantity,
      }));

    if (items.length === 0) {
      setSubmitError("Please select at least one product.");
      return;
    }

    const request: CreateOrderRequest = {
      customerName: customerName.trim(),
      items,
    };

    try {
      setSubmitting(true);

      const order = await createOrder(request);

      setSuccessMessage(`Order #${order.id} created successfully.`);

      setCustomerName("");
      setQuantities({});
    } catch (error) {
      setSubmitError(
        error instanceof Error
          ? error.message
          : "Order could not be created. Please try again."
      );
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <main className="page">
      <header className="page-header">
        <p className="eyebrow">Mini Order App</p>

        <h1>Create Order</h1>

        <p className="page-description">
          Select products and quantities to create a new order.
        </p>

        {successMessage && (
          <div className="alert alert-success">{successMessage}</div>
        )}

        {submitError && <div className="alert alert-error">{submitError}</div>}
      </header>

      <div className="order-form">
        <div className="form-group">
          <label htmlFor="customerName">Customer Name</label>

          <input
            id="customerName"
            type="text"
            className="form-input"
            value={customerName}
            onChange={(event) => setCustomerName(event.target.value)}
            placeholder="Enter customer name"
          />
        </div>

        {loading && <div className="status-message">Loading products...</div>}

        {error && <div className="status-message error">{error}</div>}

        {!loading && !error && (
          <div className="order-product-list">
            {products.map((product) => {
              const quantity = quantities[product.id] ?? 0;

              return (
                <div key={product.id} className="order-product-row">
                  <div>
                    <span className="stock-code">{product.stockCode}</span>

                    <h3>{product.name}</h3>

                    <p className="order-product-meta">
                      {product.price.toLocaleString("tr-TR")} ₺{" · "}
                      {product.stockQuantity} in stock
                    </p>
                  </div>

                  <div className="quantity-control">
                    <button
                      type="button"
                      onClick={() => updateQuantity(product.id, quantity - 1)}
                    >
                      −
                    </button>

                    <input
                      type="number"
                      min="0"
                      max={product.stockQuantity}
                      value={quantity}
                      onChange={(event) =>
                        updateQuantity(
                          product.id,
                          Math.min(
                            Number(event.target.value),
                            product.stockQuantity
                          )
                        )
                      }
                    />

                    <button
                      type="button"
                      disabled={quantity >= product.stockQuantity}
                      onClick={() => updateQuantity(product.id, quantity + 1)}
                    >
                      +
                    </button>
                  </div>
                </div>
              );
            })}
          </div>
        )}

        <div className="order-summary">
          <span>Total</span>

          <strong>{totalAmount.toLocaleString("tr-TR")} ₺</strong>
        </div>

        <button
          type="button"
          className="primary-button"
          disabled={submitting}
          onClick={handleSubmit}
        >
          {submitting ? "Creating..." : "Create Order"}
        </button>
      </div>
    </main>
  );
}

export default CreateOrderPage;
