import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { api, ApiClientError } from "../lib/api";
import { useAppStore } from "../store/appStore";
import type { TaskDetailDto } from "../types/api";

export function TaskDetailPage() {
  const { taskId = "" } = useParams();
  const realtimeTick = useAppStore((state) => state.realtimeTick);
  const [task, setTask] = useState<TaskDetailDto | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    void api.getTask(taskId)
      .then((result) => {
        setTask(result);
        setError("");
      })
      .catch((cause) => setError(cause instanceof ApiClientError ? cause.message : "Failed to load task detail."));
  }, [realtimeTick, taskId]);

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1 className="page-title">{task?.title ?? "Task detail"}</h1>
          <p className="page-description">{task?.summary ?? "Task detail view with events, runs, and approvals."}</p>
        </div>
        {task ? <span className="pill">{task.status}</span> : null}
      </div>

      {error ? <div className="panel error">{error}</div> : null}

      <div className="split">
        <div className="panel stack">
          <h2>Events</h2>
          {task ? task.events.map((item) => (
            <div key={item.eventId} className="list-item">
              <div className="list-item-header">
                <strong>{item.eventType}</strong>
                <span className="muted">{new Date(item.createdAtUtc).toLocaleString()}</span>
              </div>
              <span>{item.payloadSummary}</span>
            </div>
          )) : <p className="muted">Loading task detail…</p>}
        </div>

        <div className="panel stack">
          <h2>Runs</h2>
          {task ? task.runs.map((item) => (
            <div key={item.runId} className="list-item">
              <div className="list-item-header">
                <strong>{item.kind}</strong>
                <span className="pill">{item.status}</span>
              </div>
              <span>{item.summary}</span>
            </div>
          )) : <p className="muted">Loading task detail…</p>}
        </div>

        <div className="panel stack">
          <h2>Approvals</h2>
          {task?.approvals.length ? task.approvals.map((item) => (
            <div key={item.approvalId} className="list-item">
              <div className="list-item-header">
                <strong>{item.title}</strong>
                <span className="pill">{item.status}</span>
              </div>
              <span className="muted">{item.riskLevel} risk</span>
            </div>
          )) : <p className="muted">No approvals for this task.</p>}
        </div>
      </div>
    </section>
  );
}
