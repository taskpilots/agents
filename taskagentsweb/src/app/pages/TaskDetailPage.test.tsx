import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { describe, expect, test, vi } from "vitest";
import { TaskDetailPage } from "./TaskDetailPage";

describe("TaskDetailPage", () => {
  test("renders events runs and approvals panels", async () => {
    vi.stubGlobal("fetch", vi.fn(() => Promise.resolve(new Response(JSON.stringify({
      taskId: "task-1",
      title: "Task detail title",
      source: "manual",
      status: "Running",
      summary: "Task detail summary",
      priority: "medium",
      requiresApproval: true,
      createdAtUtc: new Date().toISOString(),
      updatedAtUtc: new Date().toISOString(),
      events: [{ eventId: "event-1", taskId: "task-1", eventType: "taskCreated", source: "manual", payloadSummary: "Created", createdAtUtc: new Date().toISOString() }],
      runs: [{ runId: "run-1", taskId: "task-1", kind: "main-agent", status: "Running", summary: "Run summary", requestedAtUtc: new Date().toISOString(), startedAtUtc: null, completedAtUtc: null, waitingReason: null }],
      approvals: [{ approvalId: "approval-1", taskId: "task-1", runId: "run-1", title: "Need approval", status: "Pending", riskLevel: "medium", requestedBy: "main-agent", decisionNote: null, requestedAtUtc: new Date().toISOString(), resolvedAtUtc: null }],
    })))));

    render(
      <MemoryRouter initialEntries={["/tasks/task-1"]}>
        <Routes>
          <Route path="/tasks/:taskId" element={<TaskDetailPage />} />
        </Routes>
      </MemoryRouter>,
    );

    await waitFor(() => {
      expect(screen.getByText("Events")).toBeInTheDocument();
      expect(screen.getByText("Runs")).toBeInTheDocument();
      expect(screen.getByText("Approvals")).toBeInTheDocument();
    });
  });
});
