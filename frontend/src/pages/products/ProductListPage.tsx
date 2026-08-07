import { useEffect, useState } from "react";
import { API_BASE_URL } from "../../api/api";
import type { Product } from "../../types/product";

function ProductListPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(search);
    }, 400);

    return () => clearTimeout(timer);
  }, [search]);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        setLoading(true);
        setError(null);

        const url = debouncedSearch.trim()
          ? `${API_BASE_URL}/products?search=${encodeURIComponent(
              debouncedSearch.trim()
            )}`
          : `${API_BASE_URL}/products`;

        const response = await fetch(url);

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
  }, [debouncedSearch]);

  return (
    <main className="page">
      <header className="page-header">
        <p className="eyebrow">Mini Order App</p>

        <h1>Products</h1>

        <p className="page-description">
          Browse available products and current stock levels.
        </p>
      </header>

      <div className="product-toolbar">
        <div className="search-wrapper">
          <input
            type="search"
            className="search-input"
            placeholder="Search by product name or stock code..."
            value={search}
            onChange={(event) => setSearch(event.target.value)}
          />
        </div>

        <span className="product-count">{products.length} products</span>
      </div>

      {loading && <div className="status-message">Loading products...</div>}

      {error && <div className="status-message error">{error}</div>}

      {!loading && !error && products.length === 0 && (
        <div className="status-message">No products found!</div>
      )}

      {products.length === 0 ? (
        <div className="status-message">No products found!</div>
      ) : (
        <div className="product-grid">
          {products.map((product) => (
            <article key={product.id} className="product-card">
              <div className="product-card-header">
                <div>
                  <span className="stock-code">{product.stockCode}</span>

                  <h2>{product.name}</h2>
                </div>

                <span
                  className={
                    product.stockQuantity > 0
                      ? "stock-badge"
                      : "stock-badge out-of-stock"
                  }
                >
                  {product.stockQuantity > 0
                    ? `${product.stockQuantity} in stock`
                    : "Out of stock"}
                </span>
              </div>

              <div className="product-card-footer">
                <span>Price</span>

                <strong>{product.price.toLocaleString("tr-TR")} ₺</strong>
              </div>
            </article>
          ))}
        </div>
      )}
    </main>
  );
}

export default ProductListPage;
