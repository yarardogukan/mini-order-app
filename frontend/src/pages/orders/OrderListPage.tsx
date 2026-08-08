import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getOrders } from "../../api/orderApi";
import type { Order } from "../../types/order";

function OrderListPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchOrders = async () => {
      try {
        setLoading(true);
        setError(null);

        const data = await getOrders();

        setOrders(data);
      } catch (error) {
        setError(
          error instanceof Error ? error.message : "Orders could not be loaded."
        );
      } finally {
        setLoading(false);
      }
    };

    fetchOrders();
  }, []);

  return (
    <main className="page">
      <header className="page-header">
        <p className="eyebrow">Mini Order App</p>

        <h1>Orders</h1>

        <p className="page-description">View previously created orders.</p>
      </header>

      {loading && <div className="status-message">Loading orders...</div>}

      {error && <div className="status-message error">{error}</div>}

      {!loading && !error && orders.length === 0 && (
        <div className="status-message">No orders found.</div>
      )}

      {!loading && !error && orders.length > 0 && (
        <div className="order-list">
          {orders.map((order) => (
            <article key={order.id} className="order-card">
              <div>
                <span className="stock-code">Order #{order.id}</span>

                <h2>{order.customerName}</h2>

                <p className="order-product-meta">
                  {new Date(order.createdAt).toLocaleString("tr-TR")}
                </p>
              </div>

              <div className="order-card-actions">
                <strong>{order.totalAmount.toLocaleString("tr-TR")} ₺</strong>

                <Link to={`/orders/${order.id}`} className="secondary-link">
                  View Details
                </Link>
              </div>
            </article>
          ))}
        </div>
      )}
    </main>
  );
}

export default OrderListPage;
