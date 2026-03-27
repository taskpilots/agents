import * as signalR from "@microsoft/signalr";
import type { ConnectionStatus } from "../store/appStore";

interface RealtimeOptions {
  onNotification: (payload: { message: string; occurredAtUtc: string }) => void;
  onStatusChange: (status: ConnectionStatus) => void;
}

export function createRealtimeConnection({ onNotification, onStatusChange }: RealtimeOptions) {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/agent-hub")
    .withAutomaticReconnect()
    .build();

  connection.on("onNotification", onNotification);
  connection.onreconnecting(() => onStatusChange("reconnecting"));
  connection.onreconnected(async () => {
    onStatusChange("connected");
    await connection.invoke("SubscribeToWorkspace", "dashboard");
  });
  connection.onclose(() => onStatusChange("closed"));

  return {
    async start() {
      await connection.start();
      onStatusChange("connected");
      await connection.invoke("SubscribeToWorkspace", "dashboard");
    },
    async stop() {
      await connection.stop();
      onStatusChange("closed");
    },
  };
}
