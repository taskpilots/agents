import type { ReactNode } from "react";
import { NavLink } from "react-router-dom";
import { useAppStore } from "../store/appStore";

const navItems = [
  { to: "/", label: "Dashboard" },
  { to: "/tasks", label: "Tasks" },
  { to: "/runs", label: "Runs" },
  { to: "/approvals", label: "Approvals" },
  { to: "/mailbox", label: "Mailbox" },
  { to: "/notifications", label: "Notifications" },
  { to: "/settings", label: "Settings" },
];

export function AppShell({ children }: { children: ReactNode }) {
  const connectionStatus = useAppStore((state) => state.connectionStatus);

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">TaskPilots.TaskAgents</div>
        <div className="subtitle">Long-running internet analysis control plane</div>
        <nav className="nav-list">
          {navItems.map((item) => (
            <NavLink key={item.to} to={item.to} className={({ isActive }) => `nav-link${isActive ? " active" : ""}`}>
              {item.label}
            </NavLink>
          ))}
        </nav>
        <div className="connection-badge">Realtime: {connectionStatus}</div>
      </aside>
      <main className="main-content">{children}</main>
    </div>
  );
}
