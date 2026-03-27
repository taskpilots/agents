import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

const backendTarget = process.env.TASK_AGENTS_API_ORIGIN ?? "http://localhost:5131";

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/api": {
        target: backendTarget,
        changeOrigin: true,
      },
      "/agent-hub": {
        target: backendTarget,
        ws: true,
        changeOrigin: true,
      },
    },
  },
});
