import { NavLink } from "react-router-dom";
import { useCart } from "../hooks/useCart";

function Navbar() {
  const { cart } = useCart();

  const cartItemCount = cart?.itemCount ?? 0;

  return (
    <header className="navbar">
      <div className="navbar-container">
        <NavLink to="/products" className="navbar-brand">
          <span className="navbar-brand-mark">M</span>
          <span>MiniOrder</span>
        </NavLink>

        <nav className="navbar-navigation">
          <NavLink
            to="/products"
            end
            className={({ isActive }) =>
              isActive ? "nav-link active" : "nav-link"
            }
          >
            Products
          </NavLink>
        </nav>

        <NavLink
          to="/cart"
          className={({ isActive }) =>
            isActive ? "navbar-cart active" : "navbar-cart"
          }
        >
          <span className="navbar-cart-icon">🛒</span>
          <span>Cart</span>

          {cartItemCount > 0 && (
            <span className="navbar-cart-badge">
              {cartItemCount > 99 ? "99+" : cartItemCount}
            </span>
          )}
        </NavLink>
      </div>
    </header>
  );
}

export default Navbar;
