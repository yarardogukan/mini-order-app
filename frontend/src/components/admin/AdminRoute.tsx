import { Navigate, Outlet } from "react-router-dom";
import { isAdminAuthenticated } from "../../utils/adminSession";

function AdminRoute() {
  if (!isAdminAuthenticated()) {
    return <Navigate to="/admin/login" replace />;
  }

  return <Outlet />;
}

export default AdminRoute;
