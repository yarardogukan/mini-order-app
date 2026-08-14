const ADMIN_SESSION_KEY = "mini-order-admin-authenticated";

export function isAdminAuthenticated(): boolean {
  return sessionStorage.getItem(ADMIN_SESSION_KEY) === "true";
}

export function createAdminSession(): void {
  sessionStorage.setItem(ADMIN_SESSION_KEY, "true");
}

export function clearAdminSession(): void {
  sessionStorage.removeItem(ADMIN_SESSION_KEY);
}
