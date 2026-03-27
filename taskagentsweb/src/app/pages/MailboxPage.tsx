import { useEffect, useState } from "react";
import { api, ApiClientError } from "../lib/api";
import { useAppStore } from "../store/appStore";
import type { MailboxMessageListItemDto } from "../types/api";

export function MailboxPage() {
  const realtimeTick = useAppStore((state) => state.realtimeTick);
  const [messages, setMessages] = useState<MailboxMessageListItemDto[]>([]);
  const [subject, setSubject] = useState("");
  const [fromAddress, setFromAddress] = useState("requestor@example.local");
  const [body, setBody] = useState("");
  const [error, setError] = useState("");

  async function loadMessages() {
    try {
      setMessages(await api.getMailboxMessages());
      setError("");
    } catch (cause) {
      setError(cause instanceof ApiClientError ? cause.message : "Failed to load mailbox.");
    }
  }

  useEffect(() => {
    void loadMessages();
  }, [realtimeTick]);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    try {
      await api.simulateMailboxIngress({ subject, fromAddress, body });
      setSubject("");
      setBody("");
      await loadMessages();
    } catch (cause) {
      setError(cause instanceof ApiClientError ? cause.message : "Failed to ingest mailbox message.");
    }
  }

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1 className="page-title">Mailbox</h1>
          <p className="page-description">Inspect archived mailbox traffic and simulate inbound messages.</p>
        </div>
      </div>

      <div className="split">
        <form className="panel stack" onSubmit={handleSubmit}>
          <h2>Simulate inbound message</h2>
          <input className="input" value={subject} onChange={(event) => setSubject(event.target.value)} placeholder="Subject" />
          <input className="input" value={fromAddress} onChange={(event) => setFromAddress(event.target.value)} placeholder="From address" />
          <textarea className="textarea" value={body} onChange={(event) => setBody(event.target.value)} placeholder="Message body" />
          <button className="button" type="submit">Ingest message</button>
        </form>

        <div className="panel stack">
          <h2>Archived messages</h2>
          {error ? <p className="error">{error}</p> : null}
          {messages.map((message) => (
            <div key={message.messageId} className="list-item">
              <div className="list-item-header">
                <strong>{message.subject}</strong>
                <span className="pill">{message.status}</span>
              </div>
              <span>{message.summary}</span>
              <span className="muted">{message.fromAddress}</span>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
