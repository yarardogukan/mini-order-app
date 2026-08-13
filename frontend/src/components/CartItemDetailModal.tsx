import { useEffect, useState } from "react";
import { getProductById } from "../api/productApi";
import { useCart } from "../hooks/useCart";
import type { ProductDetail } from "../types/product";

interface CartItemDetailModalProps {
  productId: number;
  onClose: () => void;
}

function CartItemDetailModal({ productId, onClose }: CartItemDetailModalProps) {
  const { cart, updateItemQuantity, removeItem } = useCart();

  const [product, setProduct] = useState<ProductDetail | null>(null);

  const [selectedImageUrl, setSelectedImageUrl] = useState<string | null>(null);

  const [loading, setLoading] = useState(true);
  const [imageError, setImageError] = useState(false);
  const [updatingQuantity, setUpdatingQuantity] = useState(false);
  const [removing, setRemoving] = useState(false);

  const item =
    cart?.items.find((cartItem) => cartItem.productId === productId) ?? null;

  useEffect(() => {
    const loadProduct = async () => {
      try {
        setLoading(true);
        setImageError(false);

        const data = await getProductById(productId);

        setProduct(data);

        const coverImage =
          data.images.find((image) => image.isCover) ?? data.images[0];

        setSelectedImageUrl(
          coverImage?.imageUrl ?? item?.coverImageUrl ?? null
        );
      } catch {
        setProduct(null);

        setSelectedImageUrl(item?.coverImageUrl ?? null);
      } finally {
        setLoading(false);
      }
    };

    loadProduct();
  }, [productId, item?.coverImageUrl]);

  if (!item) {
    return null;
  }

  const handleDecreaseQuantity = async () => {
    if (item.quantity <= 1 || updatingQuantity) {
      return;
    }

    try {
      setUpdatingQuantity(true);

      await updateItemQuantity(item.productId, item.quantity - 1);
    } finally {
      setUpdatingQuantity(false);
    }
  };

  const handleIncreaseQuantity = async () => {
    if (updatingQuantity) {
      return;
    }

    try {
      setUpdatingQuantity(true);

      await updateItemQuantity(item.productId, item.quantity + 1);
    } finally {
      setUpdatingQuantity(false);
    }
  };

  const handleRemove = async () => {
    if (removing) {
      return;
    }

    try {
      setRemoving(true);

      await removeItem(item.productId);

      onClose();
    } finally {
      setRemoving(false);
    }
  };

  return (
    <div className="cart-quick-view">
      <div className="cart-quick-view-media">
        <div className="cart-quick-view-image">
          {selectedImageUrl && !imageError ? (
            <img
              src={selectedImageUrl}
              alt={item.productName}
              onError={() => setImageError(true)}
            />
          ) : (
            <div className="cart-quick-view-placeholder">
              No image available
            </div>
          )}
        </div>

        {!loading && product && product.images.length > 1 && (
          <div className="cart-quick-view-thumbnails">
            {product.images
              .slice()
              .sort((a, b) => a.sortOrder - b.sortOrder)
              .map((image) => (
                <button
                  key={`${image.imageUrl}-${image.sortOrder}`}
                  type="button"
                  className={
                    selectedImageUrl === image.imageUrl
                      ? "cart-quick-view-thumbnail active"
                      : "cart-quick-view-thumbnail"
                  }
                  onClick={() => {
                    setSelectedImageUrl(image.imageUrl);

                    setImageError(false);
                  }}
                >
                  <img
                    src={image.imageUrl}
                    alt={`${item.productName} ${image.sortOrder}`}
                    onError={(event) => {
                      event.currentTarget.style.display = "none";
                    }}
                  />
                </button>
              ))}
          </div>
        )}
      </div>

      <div className="cart-quick-view-content">
        <div className="cart-quick-view-heading">
          <span className="cart-quick-view-brand">{item.brandName}</span>

          <h2>{item.productName}</h2>
        </div>

        <div className="cart-quick-view-divider" />

        <div className="cart-quick-view-meta">
          <div>
            <span>Stock Code</span>

            <strong>{item.stockCode}</strong>
          </div>

          <div>
            <span>Brand</span>

            <strong>{item.brandName}</strong>
          </div>

          <div>
            <span>Category</span>

            <strong>{item.categoryName}</strong>
          </div>

          <div>
            <span>Unit Price</span>

            <strong>{item.unitPrice.toLocaleString("tr-TR")} ₺</strong>
          </div>
        </div>

        <div className="cart-quick-view-purchase">
          <div>
            <span className="cart-quick-view-label">Quantity</span>

            <div className="cart-quick-view-quantity">
              <button
                type="button"
                disabled={item.quantity <= 1 || updatingQuantity}
                onClick={handleDecreaseQuantity}
                aria-label="Decrease quantity"
              >
                −
              </button>

              <span>{item.quantity}</span>

              <button
                type="button"
                disabled={updatingQuantity}
                onClick={handleIncreaseQuantity}
                aria-label="Increase quantity"
              >
                +
              </button>
            </div>
          </div>

          <div className="cart-quick-view-total">
            <strong>{item.lineTotal.toLocaleString("tr-TR")} ₺</strong>

            <span>Total</span>
          </div>
        </div>

        {product?.description && (
          <div className="cart-quick-view-description">
            <h3>Product Information</h3>

            <p>{product.description}</p>
          </div>
        )}

        <div className="cart-quick-view-actions">
          <button
            type="button"
            className="cart-quick-view-remove"
            disabled={removing}
            onClick={handleRemove}
          >
            {removing ? "Removing..." : "Remove from Cart"}
          </button>

          <button
            type="button"
            className="cart-quick-view-back"
            onClick={onClose}
          >
            Back to Cart
          </button>
        </div>
      </div>
    </div>
  );
}

export default CartItemDetailModal;
