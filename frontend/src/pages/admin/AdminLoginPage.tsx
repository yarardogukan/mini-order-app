import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import googleAuthenticatorIcon from "../../assets/icons/google-authenticator.svg";
import googleIcon from "../../assets/icons/google.svg";
import {
  createAdminSession,
  isAdminAuthenticated,
} from "../../utils/adminSession";

function AdminLoginPage() {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [passwordVisible, setPasswordVisible] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    useEffect(() => {
      if (isAdminAuthenticated()) {
        navigate("/admin", {
          replace: true,
        });
      }
    }, [navigate]);

    try {
      setSubmitting(true);
      setError(null);

      await Promise.resolve();

      if (username !== "admin" || password !== "admin") {
        setError("Invalid username or password.");
        return;
      }

      createAdminSession();

      navigate("/admin", {
        replace: true,
      });
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <main className="admin-login-page">
      <div
        className="admin-login-background-shape admin-login-shape-one"
        aria-hidden="true"
      />

      <div
        className="admin-login-background-shape admin-login-shape-two"
        aria-hidden="true"
      />

      <section className="admin-login-card">
        <div className="admin-login-brand">
          <div className="admin-login-logo">
            <span>M</span>
          </div>

          <h1>Welcome back</h1>

          <p>MiniOrder Admin</p>
        </div>

        <div className="admin-demo-access">
          <div className="admin-demo-access-icon">✓</div>

          <div>
            <strong>Demo Admin Access</strong>

            <span>
              Username: <code>admin</code>
            </span>

            <span>
              Password: <code>admin</code>
            </span>
          </div>
        </div>

        <form className="admin-login-form" onSubmit={handleSubmit}>
          <label className="admin-login-field">
            <span>Username</span>

            <div className="admin-login-input">
              <span className="admin-login-input-icon" aria-hidden="true">
                ◯
              </span>

              <input
                type="text"
                autoComplete="username"
                value={username}
                onChange={(event) => setUsername(event.target.value)}
                placeholder="Enter username"
                disabled={submitting}
              />
            </div>
          </label>

          <label className="admin-login-field">
            <span>Password</span>

            <div className="admin-login-input">
              <span className="admin-login-input-icon" aria-hidden="true">
                ◇
              </span>

              <input
                type={passwordVisible ? "text" : "password"}
                autoComplete="current-password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                placeholder="Enter password"
                disabled={submitting}
              />

              <button
                type="button"
                className="admin-password-toggle"
                aria-label={passwordVisible ? "Hide password" : "Show password"}
                onClick={() => setPasswordVisible((current) => !current)}
              >
                {passwordVisible ? "◉" : "◎"}
              </button>
            </div>
          </label>

          {error && (
            <div className="admin-login-error" role="alert">
              {error}
            </div>
          )}

          <button
            type="submit"
            className="admin-login-submit"
            disabled={submitting}
          >
            <span>{submitting ? "Signing in..." : "Sign In"}</span>

            {!submitting && <span aria-hidden="true">→</span>}
          </button>
        </form>

        <div className="admin-login-divider">
          <span>or continue with</span>
        </div>

        <div className="admin-login-alternative-actions">
          <button
            type="button"
            className="admin-login-provider"
            disabled
            title="Google Sign-In will be enabled with real authentication."
          >
            <img
              src={googleIcon}
              alt=""
              className="admin-login-provider-icon"
              aria-hidden="true"
            />

            <span>Google ile Giriş Yap</span>
          </button>

          <button
            type="button"
            className="admin-login-provider"
            disabled
            title="Google Authenticator 2FA will be enabled with real authentication."
          >
            <img
              src={googleAuthenticatorIcon}
              alt=""
              className="admin-login-provider-icon"
              aria-hidden="true"
            />

            <span>Google Authenticator ile Giriş Yap</span>
          </button>
        </div>

        <p className="admin-login-footnote">
          Demo authentication is enabled for the current portfolio environment.
        </p>
      </section>
    </main>
  );
}

export default AdminLoginPage;
