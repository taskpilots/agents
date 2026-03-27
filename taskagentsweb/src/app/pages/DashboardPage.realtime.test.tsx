import { render, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, test, vi } from "vitest";
import { useAppStore } from "../store/appStore";
import { DashboardPage } from "./DashboardPage";

describe("DashboardPage realtime refresh", () => {
  beforeEach(() => {
    useAppStore.setState({ realtimeTick: 0, taskSearch: "", connectionStatus: "idle" });
  });

  test("refetches after realtime tick changes", async () => {
    const fetchMock = vi.fn(() => Promise.resolve(new Response(JSON.stringify({
      totalTaskCount: 1,
      runningTaskCount: 0,
      pendingApprovalCount: 0,
      notificationCount: 0,
      mailboxMessageCount: 0,
      recentTasks: [],
      pendingApprovals: [],
      recentNotifications: [],
    }))));
    vi.stubGlobal("fetch", fetchMock);

    render(
      <MemoryRouter>
        <DashboardPage />
      </MemoryRouter>,
    );

    await waitFor(() => expect(fetchMock).toHaveBeenCalledTimes(1));
    useAppStore.getState().registerRealtimeEvent();
    await waitFor(() => expect(fetchMock).toHaveBeenCalledTimes(2));
  });
});
