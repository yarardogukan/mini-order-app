import { useEffect, useState } from "react";
import { getCategories } from "../../api/categoryApi";
import { getProducts } from "../../api/productApi";
import type { Category } from "../../types/category";
import type { Product } from "../../types/product";

function ProductListPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [categories, setCategories] = useState<Category[]>([]);
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(
    null
  );

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const data = await getCategories();

        setCategories(data);
      } catch {
        setError("Categories could not be loaded.");
      }
    };

    fetchCategories();
  }, []);

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

        const data = await getProducts({
          search: debouncedSearch,
          categoryId: selectedCategoryId,
        });

        setProducts(data);
      } catch {
        setError("Products could not be loaded.");
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, [debouncedSearch, selectedCategoryId]);

  return (
    <main className="page">
      <header className="page-header">
        <p className="eyebrow">Mini Order App</p>

        <h1>Products</h1>

        <p className="page-description">
          Browse available products and current stock levels.
        </p>
      </header>

      <div className="category-filter">
        <button
          type="button"
          className={selectedCategoryId === null ? "active" : ""}
          onClick={() => setSelectedCategoryId(null)}
        >
          All
        </button>

        {categories.map((category) => (
          <button
            key={category.id}
            type="button"
            className={selectedCategoryId === category.id ? "active" : ""}
            onClick={() => setSelectedCategoryId(category.id)}
          >
            {category.name}
          </button>
        ))}
      </div>

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

      {!loading && !error && products.length > 0 && (
        <div className="product-grid">
          {products.map((product) => (
            <article key={product.id} className="product-card">
              <div className="product-card-image">
                {product.coverImageUrl ? (
                  <img src={product.coverImageUrl} alt={product.name} />
                ) : (
                  <div className="product-card-image-placeholder">No image</div>
                )}
              </div>
              <div className="product-card-header">
                <div>
                  <span className="stock-code">{product.stockCode}</span>

                  <div className="product-card-meta">
                    <span className="product-brand">{product.brandName}</span>

                    <span className="product-category">
                      {product.categoryName}
                    </span>
                  </div>

                  <h2>{product.name}</h2>

                  <p className="product-description">{product.description}</p>
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
