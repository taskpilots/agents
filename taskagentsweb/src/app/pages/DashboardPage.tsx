import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api, ApiClientError } from "../lib/api";
import { useAppStore } from "../store/appStore";
import type { SystemSummaryDto } from "../types/api";

function MetricCard({ label, value }: { label: string; value: number }) {
  return (
    <div className="card">
      <div className="metric-label">{label}</div>
      <div className="metric-value">{value}</div>
    </div>
  );
}

export function DashboardPage() {
  const realtimeTick = useAppStore((state) => state.realtimeTick);
  const [summary, setSummary] = useState<SystemSummaryDto | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    void api.getSystemSummary()
      .then((result) => {
        setSummary(result);
        setError("");
      })
      .catch((cause) => {
        setError(cause instanceof ApiClientError ? cause.message : "Failed to load dashboard.");
      });
  }, [realtimeTick]);

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1 className="page-title">System Dashboard</h1>
          <p className="page-description">Live summary across tasks, approvals, notifications, and mailbox intake.</p>
        </div>
      </div>

      {error ? <div className="panel error">{error}</div> : null}

      <div className="card-grid">
        <MetricCard label="Total tasks" value={summary?.totalTaskCount ?? 0} />
        <MetricCard label="Active tasks" value={summary?.runningTaskCount ?? 0} />
        <MetricCard label="Pending approvals" value={summary?.pendingApprovalCount ?? 0} />
        <MetricCard label="Mailbox messages" value={summary?.mailboxMessageCount ?? 0} />
      </div>

      <div className="split">
        <div className="panel stack">
          <h2>Recent tasks</h2>
          {summary ? summary.recentTasks.map((task) => (
            <Link key={task.taskId} to={`/tasks/${task.taskId}`} className="list-item">
              <div className="list-item-header">
                <strong>{task.title}</strong>
                <span className="pill">{task.status}</span>
              </div>
              <span className="muted">{task.source}</span>
            </Link>
          )) : <p className="muted">Loading tasks…</p>}
        </div>

        <div className="panel stack">
          <h2>Pending approvals</h2>
          {summary?.pendingApprovals.length ? summary.pendingApprovals.map((approval) => (
            <Link key={approval.approvalId} to="/approvals" className="list-item">
              <div className="list-item-header">
                <strong>{approval.title}</strong>
                <span className="pill">{approval.status}</span>
              </div>
              <span className="muted">{approval.riskLevel} risk</span>
            </Link>
          )) : <p className="muted">No pending approvals.</p>}
        </div>
      </div>
    </section>
  );
}
