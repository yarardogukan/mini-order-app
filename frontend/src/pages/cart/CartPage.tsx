import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import CartItemDetailModal from "../../components/CartItemDetailModal";
import { useCart } from "../../hooks/useCart";

function CartPage() {
  const { cart, loading, error, clear, updateItemQuantity, removeItem } =
    useCart();

  const [selectedProductId, setSelectedProductId] = useState<number | null>(
    null
  );

  const [updatingProductId, setUpdatingProductId] = useState<number | null>(
    null
  );

  const [removingProductId, setRemovingProductId] = useState<number | null>(
    null
  );

  const [clearingCart, setClearingCart] = useState(false);

  const [failedImageIds, setFailedImageIds] = useState<number[]>([]);

  useEffect(() => {
    if (selectedProductId === null) {
      return;
    }

    const previousOverflow = document.body.style.overflow;

    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        setSelectedProductId(null);
      }
    };

    document.body.style.overflow = "hidden";

    window.addEventListener("keydown", handleKeyDown);

    return () => {
      document.body.style.overflow = previousOverflow;

      window.removeEventListener("keydown", handleKeyDown);
    };
  }, [selectedProductId]);

  if (loading) {
    return (
      <main className="page">
        <div className="status-message">Loading cart...</div>
      </main>
    );
  }

  if (error) {
    return (
      <main className="page">
        <div className="status-message error">{error}</div>
      </main>
    );
  }

  return (
    <main className="cart-page">
      <nav className="cart-breadcrumb" aria-label="Breadcrumb">
        <Link to="/products">Home</Link>

        <span>/</span>

        <strong>Cart</strong>
      </nav>

      <section className="cart-page-header">
        <div>
          <h1>Your Cart</h1>

          <p>Review the items in your cart.</p>
        </div>

        {cart && cart.items.length > 0 && (
          <button
            type="button"
            className="cart-clear-button"
            disabled={clearingCart}
            onClick={async () => {
              try {
                setClearingCart(true);

                await clear();
              } finally {
                setClearingCart(false);
              }
            }}
          >
            {clearingCart ? "Clearing..." : "Clear Cart"}
          </button>
        )}
      </section>

      {!cart || cart.items.length === 0 ? (
        <div className="cart-empty-state">
          <div className="cart-empty-icon">🛒</div>

          <h2>Your cart is empty</h2>

          <p>Add products to your cart and they will appear here.</p>

          <Link to="/products" className="cart-empty-action">
            Continue Shopping
          </Link>
        </div>
      ) : (
        <>
          <section className="cart-summary-strip">
            <div className="cart-summary-stat">
              <strong>{cart.itemCount}</strong>

              <span>Items</span>
            </div>

            <div className="cart-summary-stat">
              <strong>{cart.subtotal.toLocaleString("tr-TR")} ₺</strong>

              <span>Subtotal</span>
            </div>

            <div className="cart-summary-stat">
              <strong>0 ₺</strong>

              <span>Shipping</span>
            </div>

            <div className="cart-summary-stat">
              <strong>{cart.total.toLocaleString("tr-TR")} ₺</strong>

              <span>Total</span>
            </div>
          </section>

          <section className="cart-items-section">
            <div className="cart-items-header">
              <span>Product</span>
              <span>Price</span>
              <span>Quantity</span>
              <span>Total</span>
            </div>

            <div className="cart-items-list">
              {cart.items.map((item) => (
                <div key={item.productId} className="cart-item-row">
                  <button
                    type="button"
                    className="cart-item-product cart-item-product-button"
                    onClick={() => setSelectedProductId(item.productId)}
                  >
                    <div className="cart-item-image">
                      {item.coverImageUrl &&
                      !failedImageIds.includes(item.productId) ? (
                        <img
                          src={item.coverImageUrl}
                          alt={item.productName}
                          onError={() => {
                            setFailedImageIds((current) =>
                              current.includes(item.productId)
                                ? current
                                : [...current, item.productId]
                            );
                          }}
                        />
                      ) : (
                        <span>No image</span>
                      )}
                    </div>

                    <div className="cart-item-info">
                      <strong>{item.productName}</strong>

                      <span>{item.categoryName}</span>

                      <small>Stock Code: {item.stockCode}</small>
                    </div>
                  </button>

                  <div className="cart-item-price">
                    {item.unitPrice.toLocaleString("tr-TR")} ₺
                  </div>

                  <div className="cart-item-quantity">
                    <button
                      type="button"
                      disabled={
                        item.quantity <= 1 ||
                        updatingProductId === item.productId
                      }
                      onClick={async () => {
                        try {
                          setUpdatingProductId(item.productId);

                          await updateItemQuantity(
                            item.productId,
                            item.quantity - 1
                          );
                        } finally {
                          setUpdatingProductId(null);
                        }
                      }}
                    >
                      −
                    </button>

                    <span>{item.quantity}</span>

                    <button
                      type="button"
                      disabled={updatingProductId === item.productId}
                      onClick={async () => {
                        try {
                          setUpdatingProductId(item.productId);

                          await updateItemQuantity(
                            item.productId,
                            item.quantity + 1
                          );
                        } finally {
                          setUpdatingProductId(null);
                        }
                      }}
                    >
                      +
                    </button>

                    <button
                      type="button"
                      className="cart-item-remove"
                      disabled={removingProductId === item.productId}
                      onClick={async () => {
                        try {
                          setRemovingProductId(item.productId);

                          await removeItem(item.productId);
                        } finally {
                          setRemovingProductId(null);
                        }
                      }}
                    >
                      {removingProductId === item.productId
                        ? "Removing..."
                        : "Remove"}
                    </button>
                  </div>

                  <div className="cart-item-total">
                    {item.lineTotal.toLocaleString("tr-TR")} ₺
                  </div>
                </div>
              ))}
            </div>
          </section>

          <section className="cart-bottom-layout">
            <Link to="/products" className="cart-continue-shopping">
              ← Continue Shopping
            </Link>

            <aside className="cart-order-summary">
              <h2>Order Summary</h2>

              <div>
                <span>Subtotal</span>

                <strong>{cart.subtotal.toLocaleString("tr-TR")} ₺</strong>
              </div>

              <div>
                <span>Shipping</span>

                <strong>0 ₺</strong>
              </div>

              <div className="cart-order-total">
                <span>Total</span>

                <strong>{cart.total.toLocaleString("tr-TR")} ₺</strong>
              </div>

              <button type="button" className="cart-checkout-button">
                Proceed to Checkout
              </button>
            </aside>
          </section>
        </>
      )}

      {selectedProductId !== null && (
        <div
          className="cart-modal-backdrop"
          role="presentation"
          onClick={() => setSelectedProductId(null)}
        >
          <div
            className="cart-item-modal"
            role="dialog"
            aria-modal="true"
            aria-label="Cart item detail"
            onClick={(event) => event.stopPropagation()}
          >
            <button
              type="button"
              className="cart-modal-close"
              aria-label="Close"
              onClick={() => setSelectedProductId(null)}
            >
              ×
            </button>

            <CartItemDetailModal
              productId={selectedProductId}
              onClose={() => setSelectedProductId(null)}
            />
          </div>
        </div>
      )}
    </main>
  );
}

export default CartPage;
