import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getBrands } from "../../api/brandApi";
import { getCategories } from "../../api/categoryApi";
import { getProducts } from "../../api/productApi";

function AdminDashboardPage() {
  const navigate = useNavigate();

  const [totalProducts, setTotalProducts] = useState(0);
  const [totalCategories, setTotalCategories] = useState(0);
  const [totalBrands, setTotalBrands] = useState(0);
  const [activeProducts, setActiveProducts] = useState(0);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadDashboardData = async () => {
      try {
        setLoading(true);
        setError(null);

        const [products, categories, brands] = await Promise.all([
          getProducts({}),
          getCategories(),
          getBrands(),
        ]);

        setTotalProducts(products.length);

        setTotalCategories(
          categories.reduce(
            (total, category) => total + 1 + category.subCategories.length,
            0
          )
        );

        setTotalBrands(brands.length);

        setActiveProducts(products.length);
      } catch {
        setError("Dashboard data could not be loaded.");
      } finally {
        setLoading(false);
      }
    };

    loadDashboardData();
  }, []);

  return (
    <main className="admin-dashboard-page">
      <section className="admin-dashboard-header">
        <div>
          <span className="admin-dashboard-eyebrow">Overview</span>

          <h1>Dashboard</h1>

          <p>Monitor the MiniOrder catalog and manage core storefront data.</p>
        </div>
      </section>

      {error && (
        <div className="admin-dashboard-error" role="alert">
          {error}
        </div>
      )}

      <section className="admin-dashboard-stats">
        <article className="admin-stat-card">
          <span>Total Products</span>

          <strong>{loading ? "—" : totalProducts}</strong>

          <small>Catalog products</small>
        </article>

        <article className="admin-stat-card">
          <span>Total Categories</span>

          <strong>{loading ? "—" : totalCategories}</strong>

          <small>Root and subcategories</small>
        </article>

        <article className="admin-stat-card">
          <span>Total Brands</span>

          <strong>{loading ? "—" : totalBrands}</strong>

          <small>Available brands</small>
        </article>

        <article className="admin-stat-card">
          <span>Active Products</span>

          <strong>{loading ? "—" : activeProducts}</strong>

          <small>Currently visible</small>
        </article>
      </section>

      <section className="admin-dashboard-grid">
        <article className="admin-dashboard-panel">
          <div className="admin-dashboard-panel-header">
            <div>
              <h2>Quick Actions</h2>

              <p>Common catalog management tasks.</p>
            </div>
          </div>

          <div className="admin-quick-actions">
            <button type="button" onClick={() => navigate("/admin/categories")}>
              <span>Manage Categories</span>

              <span aria-hidden="true">→</span>
            </button>

            <button type="button" onClick={() => navigate("/admin/brands")}>
              <span>Manage Brands</span>

              <span aria-hidden="true">→</span>
            </button>

            <button type="button" disabled>
              <span>Manage Products</span>

              <small>Coming soon</small>
            </button>
          </div>
        </article>

        <article className="admin-dashboard-panel">
          <div className="admin-dashboard-panel-header">
            <div>
              <h2>Orders</h2>

              <p>Order management will be introduced in a future sprint.</p>
            </div>
          </div>

          <div className="admin-dashboard-placeholder">
            <strong>Order Management</strong>

            <span>Coming soon</span>
          </div>
        </article>
      </section>
    </main>
  );
}

export default AdminDashboardPage;
