export interface TaskListItemDto {
  taskId: string;
  title: string;
  source: string;
  status: string;
  priority: string;
  requiresApproval: boolean;
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface TaskEventDto {
  eventId: string;
  taskId: string | null;
  eventType: string;
  source: string;
  payloadSummary: string;
  createdAtUtc: string;
}

export interface AgentRunListItemDto {
  runId: string;
  taskId: string;
  kind: string;
  status: string;
  summary: string;
  requestedAtUtc: string;
  startedAtUtc: string | null;
  completedAtUtc: string | null;
  waitingReason: string | null;
}

export interface ApprovalListItemDto {
  approvalId: string;
  taskId: string;
  runId: string | null;
  title: string;
  status: string;
  riskLevel: string;
  requestedBy: string;
  decisionNote: string | null;
  requestedAtUtc: string;
  resolvedAtUtc: string | null;
}

export interface NotificationListItemDto {
  notificationId: string;
  taskId: string | null;
  kind: string;
  channel: string;
  status: string;
  title: string;
  message: string;
  createdAtUtc: string;
  sentAtUtc: string | null;
}

export interface MailboxMessageListItemDto {
  messageId: string;
  taskId: string | null;
  subject: string;
  fromAddress: string;
  direction: string;
  status: string;
  summary: string;
  receivedAtUtc: string;
}

export interface SystemSummaryDto {
  totalTaskCount: number;
  runningTaskCount: number;
  pendingApprovalCount: number;
  notificationCount: number;
  mailboxMessageCount: number;
  recentTasks: TaskListItemDto[];
  pendingApprovals: ApprovalListItemDto[];
  recentNotifications: NotificationListItemDto[];
}

export interface TaskDetailDto {
  taskId: string;
  title: string;
  source: string;
  status: string;
  summary: string;
  priority: string;
  requiresApproval: boolean;
  createdAtUtc: string;
  updatedAtUtc: string;
  events: TaskEventDto[];
  runs: AgentRunListItemDto[];
  approvals: ApprovalListItemDto[];
}

export interface AgentRunDetailDto {
  runId: string;
  taskId: string;
  kind: string;
  status: string;
  summary: string;
  outputSummary: string | null;
  currentStep: string | null;
  requestedAtUtc: string;
  startedAtUtc: string | null;
  completedAtUtc: string | null;
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  source: string;
  requiresApproval: boolean;
}

export interface ApprovalDecisionRequest {
  note: string;
}

export interface SimulateMailboxIngestRequest {
  subject: string;
  fromAddress: string;
  body: string;
}
