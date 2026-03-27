import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { api, ApiClientError } from "../lib/api";
import { useAppStore } from "../store/appStore";
import type { TaskListItemDto } from "../types/api";

export function TasksPage() {
  const realtimeTick = useAppStore((state) => state.realtimeTick);
  const taskSearch = useAppStore((state) => state.taskSearch);
  const setTaskSearch = useAppStore((state) => state.setTaskSearch);
  const [tasks, setTasks] = useState<TaskListItemDto[]>([]);
  const [error, setError] = useState("");
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [requiresApproval, setRequiresApproval] = useState(false);

  async function loadTasks() {
    try {
      setTasks(await api.getTasks());
      setError("");
    } catch (cause) {
      setError(cause instanceof ApiClientError ? cause.message : "Failed to load tasks.");
    }
  }

  useEffect(() => {
    void loadTasks();
  }, [realtimeTick]);

  const filteredTasks = useMemo(() => {
    const query = taskSearch.trim().toLowerCase();
    if (!query) {
      return tasks;
    }

    return tasks.filter((task) => task.title.toLowerCase().includes(query) || task.status.toLowerCase().includes(query));
  }, [taskSearch, tasks]);

  async function handleCreateTask(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    try {
      await api.createTask({
        title,
        description,
        source: "manual",
        requiresApproval,
      });
      setTitle("");
      setDescription("");
      setRequiresApproval(false);
      await loadTasks();
    } catch (cause) {
      setError(cause instanceof ApiClientError ? cause.message : "Failed to create task.");
    }
  }

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1 className="page-title">Tasks</h1>
          <p className="page-description">Track all platform tasks and create synthetic work items for scaffolding.</p>
        </div>
      </div>

      <div className="split">
        <form className="panel stack" onSubmit={handleCreateTask}>
          <h2>Create task</h2>
          <input className="input" value={title} onChange={(event) => setTitle(event.target.value)} placeholder="Task title" />
          <textarea className="textarea" value={description} onChange={(event) => setDescription(event.target.value)} placeholder="Task description" />
          <label className="muted">
            <input
              type="checkbox"
              checked={requiresApproval}
              onChange={(event) => setRequiresApproval(event.target.checked)}
              style={{ marginRight: 8 }}
            />
            Requires approval
          </label>
          <button className="button" type="submit">Create task</button>
        </form>

        <div className="panel stack">
          <h2>Task list</h2>
          <input className="input" value={taskSearch} onChange={(event) => setTaskSearch(event.target.value)} placeholder="Filter by title or status" />
          {error ? <p className="error">{error}</p> : null}
          {filteredTasks.map((task) => (
            <Link key={task.taskId} to={`/tasks/${task.taskId}`} className="list-item">
              <div className="list-item-header">
                <strong>{task.title}</strong>
                <span className="pill">{task.status}</span>
              </div>
              <span className="muted">{task.source}</span>
            </Link>
          ))}
          {filteredTasks.length === 0 ? <p className="muted">No tasks match the current filter.</p> : null}
        </div>
      </div>
    </section>
  );
}
