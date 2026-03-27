import { useEffect } from "react";
import { Navigate, Route, Routes } from "react-router-dom";
import { AppShell } from "./app/components/AppShell";
import { createRealtimeConnection } from "./app/lib/realtime";
import { ApprovalPage } from "./app/pages/ApprovalsPage";
import { DashboardPage } from "./app/pages/DashboardPage";
import { MailboxPage } from "./app/pages/MailboxPage";
import { NotificationsPage } from "./app/pages/NotificationsPage";
import { RunsPage } from "./app/pages/RunsPage";
import { SettingsPage } from "./app/pages/SettingsPage";
import { TaskDetailPage } from "./app/pages/TaskDetailPage";
import { TasksPage } from "./app/pages/TasksPage";
import { useAppStore } from "./app/store/appStore";

export function App() {
  const registerRealtimeEvent = useAppStore((state) => state.registerRealtimeEvent);
  const setConnectionStatus = useAppStore((state) => state.setConnectionStatus);

  useEffect(() => {
    const connection = createRealtimeConnection({
      onNotification: () => registerRealtimeEvent(),
      onStatusChange: setConnectionStatus,
    });

    void connection.start().catch(() => setConnectionStatus("failed"));
    return () => {
      void connection.stop();
    };
  }, [registerRealtimeEvent, setConnectionStatus]);

  return (
    <AppShell>
      <Routes>
        <Route path="/" element={<DashboardPage />} />
        <Route path="/tasks" element={<TasksPage />} />
        <Route path="/tasks/:taskId" element={<TaskDetailPage />} />
        <Route path="/runs" element={<RunsPage />} />
        <Route path="/approvals" element={<ApprovalPage />} />
        <Route path="/mailbox" element={<MailboxPage />} />
        <Route path="/notifications" element={<NotificationsPage />} />
        <Route path="/settings" element={<SettingsPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </AppShell>
  );
}
