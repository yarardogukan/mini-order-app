import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { clearAdminSession } from "../utils/adminSession";

function AdminLayout() {
  const navigate = useNavigate();

  const handleLogout = () => {
    clearAdminSession();

    navigate("/admin/login", {
      replace: true,
    });
  };

  return (
    <div className="admin-layout">
      <aside className="admin-sidebar">
        <div className="admin-sidebar-brand">
          <span className="admin-sidebar-brand-mark">M</span>

          <div>
            <strong>MiniOrder</strong>
            <span>Admin</span>
          </div>
        </div>

        <nav className="admin-sidebar-navigation">
          <div className="admin-nav-section">
            <span className="admin-nav-section-title">Overview</span>

            <NavLink
              to="/admin"
              end
              className={({ isActive }) =>
                isActive ? "admin-nav-link active" : "admin-nav-link"
              }
            >
              Dashboard
            </NavLink>
          </div>

          <div className="admin-nav-section">
            <span className="admin-nav-section-title">Catalog</span>

            <NavLink
              to="/admin/categories"
              className={({ isActive }) =>
                isActive ? "admin-nav-link active" : "admin-nav-link"
              }
            >
              Categories
            </NavLink>

            <NavLink
              to="/admin/brands"
              className={({ isActive }) =>
                isActive ? "admin-nav-link active" : "admin-nav-link"
              }
            >
              Brands
            </NavLink>
          </div>

          <div className="admin-nav-section">
            <span className="admin-nav-section-title">Sales</span>

            <span className="admin-nav-link disabled">
              Orders
              <small>Soon</small>
            </span>
          </div>
        </nav>

        <div className="admin-sidebar-footer">
          <NavLink to="/products" className="admin-sidebar-store-link">
            ← Back to Store
          </NavLink>

          <button
            type="button"
            className="admin-sidebar-logout"
            onClick={handleLogout}
          >
            Logout
          </button>
        </div>
      </aside>

      <div className="admin-main">
        <header className="admin-topbar">
          <div className="admin-topbar-title">
            <strong>MiniOrder Admin</strong>
            <span>Catalog management workspace</span>
          </div>

          <div className="admin-topbar-profile">
            <div className="admin-topbar-avatar">AD</div>

            <div className="admin-topbar-profile-info">
              <strong>Admin</strong>
              <span>Demo Account</span>
            </div>
          </div>
        </header>

        <div className="admin-content">
          <Outlet />
        </div>
      </div>
    </div>
  );
}

export default AdminLayout;
