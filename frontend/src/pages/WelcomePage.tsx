import { Link } from "react-router-dom";

function WelcomePage() {
  return (
    <main className="welcome-page">
      <section className="welcome-content">
        <p className="eyebrow">Technical Case Study</p>

        <h1>Mini Order App</h1>

        <p className="welcome-developed-by">Designed & Developed by</p>

        <h2>Doğukan Yarar</h2>

        <p className="welcome-description">
          A full-stack order management application built with ASP.NET Core,
          React and TypeScript.
        </p>

        <div className="welcome-tech">
          <span>ASP.NET Core</span>
          <span>React</span>
          <span>TypeScript</span>
          <span>SQLite</span>
        </div>

        <div className="welcome-actions">
          <Link
            to="/products"
            className="welcome-button welcome-button-primary"
          >
            Explore Application
          </Link>

          <a
            href="https://github.com/yarardogukan/mini-order-app"
            target="_blank"
            rel="noreferrer"
            className="welcome-button welcome-button-secondary"
          >
            View on GitHub
          </a>
        </div>
      </section>
    </main>
  );
}

export default WelcomePage;
