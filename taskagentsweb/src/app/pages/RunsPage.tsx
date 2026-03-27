import { useEffect, useState } from "react";
import { api, ApiClientError } from "../lib/api";
import { useAppStore } from "../store/appStore";
import type { AgentRunListItemDto } from "../types/api";

export function RunsPage() {
  const realtimeTick = useAppStore((state) => state.realtimeTick);
  const [runs, setRuns] = useState<AgentRunListItemDto[]>([]);
  const [error, setError] = useState("");

  async function loadRuns() {
    try {
      setRuns(await api.getRuns());
      setError("");
    } catch (cause) {
      setError(cause instanceof ApiClientError ? cause.message : "Failed to load runs.");
    }
  }

  useEffect(() => {
    void loadRuns();
  }, [realtimeTick]);

  async function retryRun(runId: string) {
    try {
      await api.retryRun(runId);
      await loadRuns();
    } catch (cause) {
      setError(cause instanceof ApiClientError ? cause.message : "Failed to retry run.");
    }
  }

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1 className="page-title">Runs</h1>
          <p className="page-description">Observe orchestrated run instances and manually requeue them.</p>
        </div>
      </div>

      <div className="panel stack">
        {error ? <p className="error">{error}</p> : null}
        {runs.map((run) => (
          <div key={run.runId} className="list-item">
            <div className="list-item-header">
              <strong>{run.kind}</strong>
              <span className="pill">{run.status}</span>
            </div>
            <span>{run.summary}</span>
            <div className="inline-actions">
              <span className="muted">{run.taskId}</span>
              <button className="button secondary" onClick={() => void retryRun(run.runId)} type="button">
                Retry run
              </button>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}
