import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getOrderById } from "../../api/orderApi";
import type { Order } from "../../types/order";

function OrderDetailPage() {
  const { id } = useParams();

  const [order, setOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchOrder = async () => {
      try {
        setLoading(true);
        setError(null);

        const orderId = Number(id);

        if (!orderId) {
          throw new Error("Invalid order id.");
        }

        const data = await getOrderById(orderId);

        setOrder(data);
      } catch (error) {
        setError(
          error instanceof Error ? error.message : "Order could not be loaded."
        );
      } finally {
        setLoading(false);
      }
    };

    fetchOrder();
  }, [id]);

  return (
    <main className="page">
      <header className="page-header">
        <p className="eyebrow">Mini Order App</p>

        <h1>Order Details</h1>

        <p className="page-description">
          View order information and purchased products.
        </p>
      </header>

      {loading && <div className="status-message">Loading order...</div>}

      {error && <div className="status-message error">{error}</div>}

      {!loading && !error && order && (
        <div className="order-detail">
          <div className="order-detail-header">
            <div>
              <span className="stock-code">Order #{order.id}</span>

              <h2>{order.customerName}</h2>

              <p className="order-product-meta">
                {new Date(order.createdAt).toLocaleString("tr-TR")}
              </p>
            </div>

            <strong className="order-detail-total">
              {order.totalAmount.toLocaleString("tr-TR")} ₺
            </strong>
          </div>

          <div className="order-detail-items">
            {order.items.map((item) => (
              <div key={item.productId} className="order-detail-item">
                <div>
                  <span className="stock-code">{item.stockCode}</span>

                  <h3>{item.productName}</h3>

                  <p className="order-product-meta">
                    {item.quantity} × {item.unitPrice.toLocaleString("tr-TR")} ₺
                  </p>
                </div>

                <strong>{item.lineTotal.toLocaleString("tr-TR")} ₺</strong>
              </div>
            ))}
          </div>

          <div className="order-summary">
            <span>Total</span>

            <strong>{order.totalAmount.toLocaleString("tr-TR")} ₺</strong>
          </div>

          <Link to="/orders" className="secondary-link">
            Back to Orders
          </Link>
        </div>
      )}
    </main>
  );
}

export default OrderDetailPage;
