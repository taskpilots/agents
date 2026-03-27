import { useEffect, useState } from "react";
import { api, ApiClientError } from "../lib/api";
import { useAppStore } from "../store/appStore";
import type { NotificationListItemDto } from "../types/api";

export function NotificationsPage() {
  const realtimeTick = useAppStore((state) => state.realtimeTick);
  const [notifications, setNotifications] = useState<NotificationListItemDto[]>([]);
  const [error, setError] = useState("");

  useEffect(() => {
    void api.getNotifications()
      .then((result) => {
        setNotifications(result);
        setError("");
      })
      .catch((cause) => setError(cause instanceof ApiClientError ? cause.message : "Failed to load notifications."));
  }, [realtimeTick]);

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1 className="page-title">Notifications</h1>
          <p className="page-description">Observe in-app notifications emitted by approvals and run transitions.</p>
        </div>
      </div>

      <div className="panel stack">
        {error ? <p className="error">{error}</p> : null}
        {notifications.map((notification) => (
          <div key={notification.notificationId} className="list-item">
            <div className="list-item-header">
              <strong>{notification.title}</strong>
              <span className="pill">{notification.status}</span>
            </div>
            <span>{notification.message}</span>
            <span className="muted">{notification.channel}</span>
          </div>
        ))}
      </div>
    </section>
  );
}
