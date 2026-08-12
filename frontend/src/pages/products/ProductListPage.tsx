import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getBrands } from "../../api/brandApi";
import { getCategories } from "../../api/categoryApi";
import { getProducts } from "../../api/productApi";
import type { Brand } from "../../types/brand";
import type { Category } from "../../types/category";
import type { Product } from "../../types/product";

function ProductListPage() {
  const navigate = useNavigate();
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [categories, setCategories] = useState<Category[]>([]);
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(
    null
  );
  const [brands, setBrands] = useState<Brand[]>([]);
  const [selectedBrandId, setSelectedBrandId] = useState<number | null>(null);

  const [sort, setSort] = useState("nameAsc");

  const [minPrice, setMinPrice] = useState("");
  const [maxPrice, setMaxPrice] = useState("");

  const [appliedMinPrice, setAppliedMinPrice] = useState<number | null>(null);
  const [appliedMaxPrice, setAppliedMaxPrice] = useState<number | null>(null);

  const [categoriesOpen, setCategoriesOpen] = useState(true);
  const [brandsOpen, setBrandsOpen] = useState(true);
  const [priceOpen, setPriceOpen] = useState(true);
  const [openCategoryIds, setOpenCategoryIds] = useState<number[]>([]);
  const [failedImageIds, setFailedImageIds] = useState<number[]>([]);

  const selectedCategoryName =
    categories
      .flatMap((category) => [category, ...category.subCategories])
      .find((category) => category.id === selectedCategoryId)?.name ?? null;

  const selectedBrandName =
    brands.find((brand) => brand.id === selectedBrandId)?.name ?? null;

  const toggleCategory = (categoryId: number) => {
    setOpenCategoryIds((current) =>
      current.includes(categoryId)
        ? current.filter((id) => id !== categoryId)
        : [...current, categoryId]
    );
  };

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
    const fetchBrands = async () => {
      try {
        const data = await getBrands();

        setBrands(data);
      } catch {
        setError("Brands could not be loaded.");
      }
    };

    fetchBrands();
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
          brandId: selectedBrandId,
          minPrice: appliedMinPrice,
          maxPrice: appliedMaxPrice,
          sort,
        });

        setProducts(data);
      } catch {
        setError("Products could not be loaded.");
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, [
    debouncedSearch,
    selectedCategoryId,
    selectedBrandId,
    appliedMinPrice,
    appliedMaxPrice,
    sort,
  ]);

  return (
    <main className="marketplace-page">
      {/* =========================
          Marketplace Header
         ========================= */}
      <section className="marketplace-header">
        <div>
          <p className="eyebrow">Mini Order Marketplace</p>

          <h1>Products</h1>

          <p className="marketplace-description">
            Discover products across categories, brands and price ranges.
          </p>
        </div>
      </section>

      {/* =========================
          Marketplace Layout
         ========================= */}
      <section className="marketplace-content">
        {/* =========================
            Marketplace Sidebar
           ========================= */}
        <aside className="marketplace-sidebar">
          {/* Categories */}
          <div className="filter-section">
            <button
              type="button"
              className="filter-section-header"
              onClick={() => setCategoriesOpen((current) => !current)}
              aria-expanded={categoriesOpen}
            >
              <span>Categories</span>

              <span
                className={
                  categoriesOpen ? "filter-chevron open" : "filter-chevron"
                }
              >
                ⌄
              </span>
            </button>

            {categoriesOpen && (
              <div className="filter-section-content">
                <button
                  type="button"
                  className={
                    selectedCategoryId === null
                      ? "filter-option active"
                      : "filter-option"
                  }
                  onClick={() => setSelectedCategoryId(null)}
                >
                  All Categories
                </button>

                {categories.map((category) => {
                  const isOpen = openCategoryIds.includes(category.id);
                  const hasSubCategories = category.subCategories.length > 0;

                  return (
                    <div key={category.id} className="category-group">
                      <div className="category-row">
                        <button
                          type="button"
                          className={
                            selectedCategoryId === category.id
                              ? "filter-option category-root active"
                              : "filter-option category-root"
                          }
                          onClick={() => setSelectedCategoryId(category.id)}
                        >
                          {category.name}
                        </button>

                        {hasSubCategories && (
                          <button
                            type="button"
                            className="category-toggle"
                            onClick={() => toggleCategory(category.id)}
                            aria-label={`${category.name} subcategories`}
                            aria-expanded={isOpen}
                          >
                            <span
                              className={
                                isOpen
                                  ? "category-toggle-icon open"
                                  : "category-toggle-icon"
                              }
                            >
                              ›
                            </span>
                          </button>
                        )}
                      </div>

                      {hasSubCategories && isOpen && (
                        <div className="subcategory-list">
                          {category.subCategories.map((subCategory) => (
                            <button
                              key={subCategory.id}
                              type="button"
                              className={
                                selectedCategoryId === subCategory.id
                                  ? "filter-option subcategory active"
                                  : "filter-option subcategory"
                              }
                              onClick={() =>
                                setSelectedCategoryId(subCategory.id)
                              }
                            >
                              {subCategory.name}
                            </button>
                          ))}
                        </div>
                      )}
                    </div>
                  );
                })}
              </div>
            )}
          </div>

          {/* Brands */}
          <div className="filter-section">
            <button
              type="button"
              className="filter-section-header"
              onClick={() => setBrandsOpen((current) => !current)}
              aria-expanded={brandsOpen}
            >
              <span>Brands</span>

              <span
                className={
                  brandsOpen ? "filter-chevron open" : "filter-chevron"
                }
              >
                ⌄
              </span>
            </button>

            {brandsOpen && (
              <div className="filter-section-content">
                <button
                  type="button"
                  className={
                    selectedBrandId === null
                      ? "filter-option active"
                      : "filter-option"
                  }
                  onClick={() => setSelectedBrandId(null)}
                >
                  All Brands
                </button>

                {brands.map((brand) => (
                  <button
                    key={brand.id}
                    type="button"
                    className={
                      selectedBrandId === brand.id
                        ? "filter-option active"
                        : "filter-option"
                    }
                    onClick={() => setSelectedBrandId(brand.id)}
                  >
                    {brand.name}
                  </button>
                ))}
              </div>
            )}
          </div>

          {/* Price Range */}
          <div className="filter-section">
            <button
              type="button"
              className="filter-section-header"
              onClick={() => setPriceOpen((current) => !current)}
              aria-expanded={priceOpen}
            >
              <span>Price Range</span>

              <span
                className={priceOpen ? "filter-chevron open" : "filter-chevron"}
              >
                ⌄
              </span>
            </button>

            {priceOpen && (
              <div className="filter-section-content">
                <div className="price-filter">
                  <div className="price-input-group">
                    <div className="price-input-wrapper">
                      <input
                        type="text"
                        inputMode="decimal"
                        placeholder="Min Price"
                        value={minPrice}
                        onChange={(event) => {
                          const value = event.target.value.replace(
                            /[^\d.,]/g,
                            ""
                          );

                          setMinPrice(value);
                        }}
                      />

                      <span className="price-currency">₺</span>
                    </div>

                    <div className="price-input-wrapper">
                      <input
                        type="text"
                        inputMode="decimal"
                        placeholder="Max Price"
                        value={maxPrice}
                        onChange={(event) => {
                          const value = event.target.value.replace(
                            /[^\d.,]/g,
                            ""
                          );

                          setMaxPrice(value);
                        }}
                      />

                      <span className="price-currency">₺</span>
                    </div>
                  </div>

                  <button
                    type="button"
                    className="apply-price-button"
                    onClick={() => {
                      const parsedMin = minPrice
                        ? Number(minPrice.replace(",", "."))
                        : null;

                      const parsedMax = maxPrice
                        ? Number(maxPrice.replace(",", "."))
                        : null;

                      setAppliedMinPrice(parsedMin);
                      setAppliedMaxPrice(parsedMax);
                    }}
                  >
                    Apply
                  </button>
                </div>
              </div>
            )}
          </div>

          {/* Clear Filters */}
          <button
            type="button"
            className="clear-filters-button"
            onClick={() => {
              setSearch("");
              setSelectedCategoryId(null);
              setSelectedBrandId(null);

              setMinPrice("");
              setMaxPrice("");

              setAppliedMinPrice(null);
              setAppliedMaxPrice(null);

              setSort("nameAsc");
            }}
          >
            Clear All Filters
          </button>
        </aside>

        {/* =========================
            Marketplace Main
           ========================= */}
        <div className="marketplace-main">
          {/* Search + Sort */}
          <section className="marketplace-toolbar">
            <div className="marketplace-search">
              <input
                type="search"
                className="search-input"
                placeholder="Search by product name, stock code, brand..."
                value={search}
                onChange={(event) => setSearch(event.target.value)}
              />
            </div>

            <div className="marketplace-sort">
              <label htmlFor="product-sort">Sort by</label>

              <select
                id="product-sort"
                value={sort}
                onChange={(event) => setSort(event.target.value)}
              >
                <option value="nameAsc">Name: A to Z</option>

                <option value="nameDesc">Name: Z to A</option>

                <option value="priceAsc">Price: Low to High</option>

                <option value="priceDesc">Price: High to Low</option>

                <option value="stockDesc">Stock: High to Low</option>
              </select>
            </div>
          </section>

          {/* =========================
              Active Filters
             ========================= */}
          <section className="active-filters">
            <div className="active-filters-content">
              <span className="active-filters-title">Active Filters:</span>

              <div className="active-filter-list">
                {selectedCategoryName && (
                  <button
                    type="button"
                    className="active-filter-chip"
                    onClick={() => setSelectedCategoryId(null)}
                  >
                    Category: {selectedCategoryName}
                    <span>×</span>
                  </button>
                )}

                {selectedBrandName && (
                  <button
                    type="button"
                    className="active-filter-chip"
                    onClick={() => setSelectedBrandId(null)}
                  >
                    Brand: {selectedBrandName}
                    <span>×</span>
                  </button>
                )}

                {(appliedMinPrice !== null || appliedMaxPrice !== null) && (
                  <button
                    type="button"
                    className="active-filter-chip"
                    onClick={() => {
                      setMinPrice("");
                      setMaxPrice("");

                      setAppliedMinPrice(null);
                      setAppliedMaxPrice(null);
                    }}
                  >
                    Price:{" "}
                    {appliedMinPrice !== null
                      ? `${appliedMinPrice.toLocaleString("tr-TR")} ₺`
                      : "0 ₺"}
                    {" - "}
                    {appliedMaxPrice !== null
                      ? `${appliedMaxPrice.toLocaleString("tr-TR")} ₺`
                      : "No limit"}
                    <span>×</span>
                  </button>
                )}

                {selectedCategoryId === null &&
                  selectedBrandId === null &&
                  appliedMinPrice === null &&
                  appliedMaxPrice === null && (
                    <span className="no-active-filters">No active filters</span>
                  )}
              </div>

              {(selectedCategoryId !== null ||
                selectedBrandId !== null ||
                appliedMinPrice !== null ||
                appliedMaxPrice !== null) && (
                <button
                  type="button"
                  className="active-filters-clear"
                  onClick={() => {
                    setSelectedCategoryId(null);
                    setSelectedBrandId(null);

                    setMinPrice("");
                    setMaxPrice("");

                    setAppliedMinPrice(null);
                    setAppliedMaxPrice(null);
                  }}
                >
                  Clear all
                </button>
              )}
            </div>

            <div className="active-filters-result-count">
              <strong>{products.length}</strong>
              <span> products found</span>
            </div>
          </section>

          {/* =========================
              Product Results
             ========================= */}
          <div className="marketplace-results">
            {loading && (
              <div className="status-message">Loading products...</div>
            )}

            {error && <div className="status-message error">{error}</div>}

            {!loading && !error && products.length === 0 && (
              <div className="status-message">
                No products match the selected filters.
              </div>
            )}

            {!loading && !error && products.length > 0 && (
              <div className="marketplace-product-grid">
                {products.map((product) => (
                  <article
                    key={product.id}
                    className="marketplace-product-card"
                    role="link"
                    tabIndex={0}
                    onClick={() => navigate(`/products/${product.id}`)}
                    onKeyDown={(event) => {
                      if (event.key === "Enter" || event.key === " ") {
                        navigate(`/products/${product.id}`);
                      }
                    }}
                  >
                    <div className="marketplace-product-image">
                      {product.coverImageUrl &&
                      !failedImageIds.includes(product.id) ? (
                        <img
                          src={product.coverImageUrl}
                          alt={product.name}
                          onError={() => {
                            setFailedImageIds((current) =>
                              current.includes(product.id)
                                ? current
                                : [...current, product.id]
                            );
                          }}
                        />
                      ) : (
                        <div className="product-card-image-placeholder">
                          <span>No image available</span>
                        </div>
                      )}
                    </div>

                    <div className="marketplace-product-body">
                      <div className="marketplace-product-heading">
                        <span className="marketplace-product-brand">
                          {product.brandName}
                        </span>

                        <h2>{product.name}</h2>

                        <span className="marketplace-stock-code">
                          Stock Code: {product.stockCode}
                        </span>
                      </div>

                      <p className="marketplace-product-description">
                        {product.description}
                      </p>

                      <div className="marketplace-product-footer">
                        <strong className="marketplace-product-price">
                          {product.price.toLocaleString("tr-TR")} ₺
                        </strong>

                        <button
                          type="button"
                          className="marketplace-cart-button"
                          aria-label={`Add ${product.name} to cart`}
                          onClick={(event) => {
                            event.stopPropagation();
                          }}
                        >
                          🛒
                        </button>
                      </div>
                    </div>
                  </article>
                ))}
              </div>
            )}
          </div>
        </div>
      </section>
    </main>
  );
}

export default ProductListPage;
