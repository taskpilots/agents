import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, test, vi } from "vitest";
import { App } from "./App";

vi.mock("./app/lib/realtime", () => ({
  createRealtimeConnection: () => ({
    start: vi.fn().mockResolvedValue(undefined),
    stop: vi.fn().mockResolvedValue(undefined),
  }),
}));

function installFetchMock() {
  vi.stubGlobal("fetch", vi.fn((input: RequestInfo | URL) => {
    const path = typeof input === "string" ? input : input.toString();
    if (path.includes("/api/system/summary")) {
      return Promise.resolve(new Response(JSON.stringify({
        totalTaskCount: 2,
        runningTaskCount: 1,
        pendingApprovalCount: 1,
        notificationCount: 1,
        mailboxMessageCount: 1,
        recentTasks: [],
        pendingApprovals: [],
        recentNotifications: [],
      })));
    }

    return Promise.resolve(new Response(JSON.stringify([])));
  }));
}

describe("App routes", () => {
  beforeEach(() => {
    installFetchMock();
  });

  test("renders dashboard route", async () => {
    render(
      <MemoryRouter initialEntries={["/"]}>
        <App />
      </MemoryRouter>,
    );

    await waitFor(() => expect(screen.getByText("System Dashboard")).toBeInTheDocument());
  });

  test("renders tasks route", async () => {
    render(
      <MemoryRouter initialEntries={["/tasks"]}>
        <App />
      </MemoryRouter>,
    );

    await waitFor(() => expect(screen.getByRole("heading", { name: "Tasks" })).toBeInTheDocument());
  });

  test("renders approvals route", async () => {
    render(
      <MemoryRouter initialEntries={["/approvals"]}>
        <App />
      </MemoryRouter>,
    );

    await waitFor(() => expect(screen.getByRole("heading", { name: "Approvals" })).toBeInTheDocument());
  });
});
