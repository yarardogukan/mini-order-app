import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getProductById } from "../../api/productApi";
import type { ProductDetail } from "../../types/product";

function ProductDetailPage() {
  const { id } = useParams();

  const [product, setProduct] = useState<ProductDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedImageUrl, setSelectedImageUrl] = useState<string | null>(null);
  const [imageError, setImageError] = useState(false);
  const [quantity, setQuantity] = useState(1);

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        setLoading(true);
        setError(null);

        const productId = Number(id);

        if (!productId) {
          setError("Invalid product.");
          return;
        }

        const data = await getProductById(productId);

        setProduct(data);

        const coverImage =
          data.images.find((image) => image.isCover) ?? data.images[0];

        setSelectedImageUrl(coverImage?.imageUrl ?? null);
      } catch {
        setError("Product could not be loaded.");
      } finally {
        setLoading(false);
      }
    };

    fetchProduct();
  }, [id]);

  if (loading) {
    return (
      <main className="page">
        <div className="status-message">Loading product...</div>
      </main>
    );
  }

  if (error || !product) {
    return (
      <main className="page">
        <div className="status-message error">
          {error ?? "Product could not be loaded."}
        </div>
      </main>
    );
  }

  return (
    <main className="page product-detail-page">
      <Link to="/products" className="back-to-products">
        ← Back to Products
      </Link>
      <nav className="product-breadcrumb" aria-label="Breadcrumb">
        <span>Products</span>

        {product.parentCategoryName && (
          <>
            <span className="breadcrumb-separator">/</span>
            <span>{product.parentCategoryName}</span>
          </>
        )}

        <span className="breadcrumb-separator">/</span>
        <span>{product.categoryName}</span>

        <span className="breadcrumb-separator">/</span>
        <strong>{product.name}</strong>
      </nav>

      <section className="product-detail-layout">
        <div className="product-detail-media">
          <div className="product-main-image">
            {selectedImageUrl && !imageError ? (
              <img
                src={selectedImageUrl}
                alt={product.name}
                onError={() => setImageError(true)}
              />
            ) : (
              <div className="product-image-placeholder">
                No image available
              </div>
            )}
          </div>
          {product.images.length > 1 && (
            <div className="product-thumbnails">
              {product.images
                .slice()
                .sort((a, b) => a.sortOrder - b.sortOrder)
                .map((image) => (
                  <button
                    key={`${image.imageUrl}-${image.sortOrder}`}
                    type="button"
                    className={
                      selectedImageUrl === image.imageUrl
                        ? "product-thumbnail active"
                        : "product-thumbnail"
                    }
                    onClick={() => {
                      setSelectedImageUrl(image.imageUrl);
                      setImageError(false);
                    }}
                  >
                    <img
                      src={image.imageUrl}
                      alt={`${product.name} ${image.sortOrder}`}
                    />
                  </button>
                ))}
            </div>
          )}
        </div>

        <div className="product-detail-content">
          <span className="product-detail-category">
            {product.categoryName}
          </span>

          <h1>{product.name}</h1>

          <p className="product-detail-description">{product.description}</p>

          <div className="product-detail-price">
            {product.price.toLocaleString("tr-TR")} ₺
          </div>

          <div className="product-detail-meta">
            <div>
              <span>Stock Code</span>
              <strong>{product.stockCode}</strong>
            </div>

            <div>
              <span>Brand</span>
              <strong>{product.brandName}</strong>
            </div>

            <div>
              <span>Category</span>
              <strong>
                {product.parentCategoryName
                  ? `${product.parentCategoryName} > ${product.categoryName}`
                  : product.categoryName}
              </strong>
            </div>

            <div>
              <span>Stock</span>
              <strong
                className={
                  product.stockQuantity > 0
                    ? "product-detail-stock"
                    : "product-detail-stock out-of-stock"
                }
              >
                {product.stockQuantity > 0
                  ? `In Stock (${product.stockQuantity} available)`
                  : "Out of stock"}
              </strong>
            </div>
            {product.attributes.length > 0 && (
              <section className="product-specifications">
                <h2>Technical Specifications</h2>

                <div className="product-specification-list">
                  {product.attributes
                    .slice()
                    .sort((a, b) => a.sortOrder - b.sortOrder)
                    .map((attribute) => (
                      <div
                        key={attribute.code}
                        className="product-specification-row"
                      >
                        <span>{attribute.name}</span>
                        <strong>{attribute.value}</strong>
                      </div>
                    ))}
                </div>
              </section>
            )}
            <div className="product-purchase">
              <div className="quantity-control">
                <button
                  type="button"
                  disabled={quantity <= 1}
                  onClick={() =>
                    setQuantity((current) => Math.max(1, current - 1))
                  }
                >
                  −
                </button>

                <span>{quantity}</span>

                <button
                  type="button"
                  disabled={quantity >= product.stockQuantity}
                  onClick={() =>
                    setQuantity((current) =>
                      Math.min(product.stockQuantity, current + 1)
                    )
                  }
                >
                  +
                </button>
              </div>

              <button
                type="button"
                className="primary-button add-to-cart-button"
                disabled={product.stockQuantity === 0}
              >
                {product.stockQuantity > 0 ? "Add to Cart" : "Out of Stock"}
              </button>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}

export default ProductDetailPage;
