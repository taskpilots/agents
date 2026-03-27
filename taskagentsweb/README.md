# TaskPilots TaskAgents Web Console

React + Vite frontend for the TaskPilots TaskAgents management console.

## Local Development

Install dependencies once:

```bash
pnpm install
```

Start the frontend dev server:

```bash
pnpm dev
```

The Vite dev server proxies `/api/*` and `/agent-hub` to `http://localhost:5131` by default, which matches the backend launch settings. Override this with `TASK_AGENTS_API_ORIGIN` if your backend runs elsewhere.

## Build And Publish To Backend

Create a production build and copy it into the backend static files directory:

```bash
pnpm build
```

`pnpm build` writes the Vite output to `dist/` and then copies it into `../TaskPilots.TaskAgents.WebApis/wwwroot`, so the backend can serve the compiled frontend directly.

If you only need to re-copy an existing build output:

```bash
pnpm run copy-wwwroot
```

## Validation

Run the frontend test suite:

```bash
pnpm test
```

Run a type check:

```bash
pnpm run typecheck
```
