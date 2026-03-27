export function SettingsPage() {
  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1 className="page-title">Settings</h1>
          <p className="page-description">Static placeholder for runtime policy, storage, and integration settings.</p>
        </div>
      </div>

      <div className="card-grid">
        <div className="card">
          <div className="metric-label">Repository mode</div>
          <div className="metric-value">InMemory</div>
        </div>
        <div className="card">
          <div className="metric-label">OpenAI mode</div>
          <div className="metric-value">Fake adapter</div>
        </div>
        <div className="card">
          <div className="metric-label">Mailbox mode</div>
          <div className="metric-value">Simulated</div>
        </div>
      </div>
    </section>
  );
}
