import type {
  AgentRunDetailDto,
  AgentRunListItemDto,
  ApprovalDecisionRequest,
  ApprovalListItemDto,
  CreateTaskRequest,
  MailboxMessageListItemDto,
  NotificationListItemDto,
  SimulateMailboxIngestRequest,
  SystemSummaryDto,
  TaskDetailDto,
  TaskEventDto,
  TaskListItemDto,
} from "../types/api";

interface ApiErrorPayload {
  msg?: string;
  code?: string;
}

export class ApiClientError extends Error {
  readonly code: string;
  readonly status: number;

  constructor(
    message: string,
    code: string,
    status: number,
  ) {
    super(message);
    this.code = code;
    this.status = status;
  }
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(path, {
    headers: {
      "Content-Type": "application/json",
      ...(init?.headers ?? {}),
    },
    ...init,
  });

  if (!response.ok) {
    let payload: ApiErrorPayload | undefined;
    try {
      payload = (await response.json()) as ApiErrorPayload;
    } catch {
      payload = undefined;
    }

    throw new ApiClientError(
      payload?.msg ?? `Request failed with status ${response.status}.`,
      payload?.code ?? "request_failed",
      response.status,
    );
  }

  return (await response.json()) as T;
}

export const api = {
  getSystemSummary: () => request<SystemSummaryDto>("/api/system/summary"),
  getTasks: () => request<TaskListItemDto[]>("/api/tasks"),
  getTask: (taskId: string) => request<TaskDetailDto>(`/api/tasks/${taskId}`),
  createTask: (payload: CreateTaskRequest) =>
    request<TaskDetailDto>("/api/tasks", { method: "POST", body: JSON.stringify(payload) }),
  getEvents: (taskId?: string) => request<TaskEventDto[]>(taskId ? `/api/events?taskId=${encodeURIComponent(taskId)}` : "/api/events"),
  getRuns: () => request<AgentRunListItemDto[]>("/api/runs"),
  getRun: (runId: string) => request<AgentRunDetailDto>(`/api/runs/${runId}`),
  retryRun: (runId: string) => request<AgentRunDetailDto>(`/api/runs/${runId}/retry`, { method: "POST" }),
  getApprovals: () => request<ApprovalListItemDto[]>("/api/approvals"),
  approveApproval: (approvalId: string, payload: ApprovalDecisionRequest) =>
    request<ApprovalListItemDto>(`/api/approvals/${approvalId}/approve`, { method: "POST", body: JSON.stringify(payload) }),
  rejectApproval: (approvalId: string, payload: ApprovalDecisionRequest) =>
    request<ApprovalListItemDto>(`/api/approvals/${approvalId}/reject`, { method: "POST", body: JSON.stringify(payload) }),
  getNotifications: () => request<NotificationListItemDto[]>("/api/notifications"),
  getMailboxMessages: () => request<MailboxMessageListItemDto[]>("/api/mailbox/messages"),
  simulateMailboxIngress: (payload: SimulateMailboxIngestRequest) =>
    request<MailboxMessageListItemDto>("/api/mailbox/simulate-ingest", { method: "POST", body: JSON.stringify(payload) }),
};
