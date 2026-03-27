import { create } from "zustand";

export type ConnectionStatus = "idle" | "connected" | "reconnecting" | "failed" | "closed";

interface AppState {
  realtimeTick: number;
  taskSearch: string;
  connectionStatus: ConnectionStatus;
  registerRealtimeEvent: () => void;
  setTaskSearch: (value: string) => void;
  setConnectionStatus: (value: ConnectionStatus) => void;
}

export const useAppStore = create<AppState>((set) => ({
  realtimeTick: 0,
  taskSearch: "",
  connectionStatus: "idle",
  registerRealtimeEvent: () => set((state) => ({ realtimeTick: state.realtimeTick + 1 })),
  setTaskSearch: (value) => set({ taskSearch: value }),
  setConnectionStatus: (value) => set({ connectionStatus: value }),
}));
