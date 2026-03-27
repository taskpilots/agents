import { useEffect, useState } from "react";
import { api, ApiClientError } from "../lib/api";
import { useAppStore } from "../store/appStore";
import type { ApprovalListItemDto } from "../types/api";

export function ApprovalPage() {
  const realtimeTick = useAppStore((state) => state.realtimeTick);
  const [approvals, setApprovals] = useState<ApprovalListItemDto[]>([]);
  const [error, setError] = useState("");

  async function loadApprovals() {
    try {
      setApprovals(await api.getApprovals());
      setError("");
    } catch (cause) {
      setError(cause instanceof ApiClientError ? cause.message : "Failed to load approvals.");
    }
  }

  useEffect(() => {
    void loadApprovals();
  }, [realtimeTick]);

  async function decide(approvalId: string, decision: "approve" | "reject") {
    try {
      if (decision === "approve") {
        await api.approveApproval(approvalId, { note: "Approved from scaffold console." });
      } else {
        await api.rejectApproval(approvalId, { note: "Rejected from scaffold console." });
      }
      await loadApprovals();
    } catch (cause) {
      setError(cause instanceof ApiClientError ? cause.message : "Failed to update approval.");
    }
  }

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1 className="page-title">Approvals</h1>
          <p className="page-description">Resolve approval gates and drive downstream task state changes.</p>
        </div>
      </div>

      <div className="panel stack">
        {error ? <p className="error">{error}</p> : null}
        {approvals.map((approval) => (
          <div key={approval.approvalId} className="list-item">
            <div className="list-item-header">
              <strong>{approval.title}</strong>
              <span className="pill">{approval.status}</span>
            </div>
            <span className="muted">{approval.requestedBy} · {approval.riskLevel} risk</span>
            <div className="inline-actions">
              <button className="button" onClick={() => void decide(approval.approvalId, "approve")} type="button">
                Approve
              </button>
              <button className="button secondary" onClick={() => void decide(approval.approvalId, "reject")} type="button">
                Reject
              </button>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}
