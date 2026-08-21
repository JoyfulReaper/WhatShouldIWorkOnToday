# What WSIWOT Is

What Should I Work On Today (WSIWOT) is a deliberately small, single-user planning tool. SQLite is authoritative. The separate private GitHub sync repository is only a planning-state mirror and an asynchronous command mailbox. Never treat `state/snapshot.json` as the database.

# Reading State

Read `state/snapshot.json` before issuing a command:

1. Fetch the latest snapshot from the configured sync branch.
2. Identify the exact WorkItem and Todo IDs in that snapshot.
3. Never guess IDs or rely on fuzzy name matching when an ID is required.
4. Check current priority and lifecycle fields before choosing a command.

The snapshot mirrors planning state. Do not edit it directly.

# Writing Commands

Write one logical command to `commands/pending/<guid>.json`. Use a fresh GUID, and make the filename GUID exactly match the JSON `id`. Keep `schemaVersion` at `1`.

Successful or failed processing produces `commands/applied/<guid>.json`; inspect its receipt before assuming the operation succeeded. Invalid command filenames may be quarantined under `commands/rejected/`. Inspect rejected errors and correct the cause instead of blindly retrying.

The receipt is written before the pending file is deleted. Command IDs are durably idempotent, and recovery can recreate a missing remote receipt without reapplying its SQLite mutation. If GitHub branch movement or a 409 conflict prevents file creation, refresh repository state before retrying. Do not create a second logical command if the first command may already exist or have been applied.

# Supported Commands

Defaults are `Project` for WorkItem kind, `Medium` for energy, `Medium` for effort, and `Normal` for WorkItem and Todo priority. Priority accepts only `Low`, `Normal`, and `High`, case-insensitively.

## createWorkItem

The optional `todos` array creates the WorkItem and its initial Todos atomically. It may be omitted, `null`, or empty. At most 100 initial Todos are accepted.

```json
{
  "schemaVersion": 1,
  "id": "11111111-1111-4111-8111-111111111111",
  "type": "createWorkItem",
  "createdAtUtc": "2026-08-21T12:00:00Z",
  "payload": {
    "name": "HappyThing",
    "kind": "Project",
    "priority": "High",
    "description": "Example",
    "url": null,
    "todos": [
      {
        "task": "Implement first thing",
        "energy": "Medium",
        "effort": "Short",
        "priority": "High"
      },
      {
        "task": "Test it",
        "energy": "Low",
        "effort": "Short",
        "priority": "Normal"
      }
    ]
  }
}
```

## createTodo

```json
{
  "schemaVersion": 1,
  "id": "22222222-2222-4222-8222-222222222222",
  "type": "createTodo",
  "createdAtUtc": "2026-08-21T12:05:00Z",
  "payload": {
    "workItemId": 4,
    "task": "Review configuration",
    "energy": "Low",
    "effort": "Short",
    "priority": "Normal"
  }
}
```

## completeTodo

```json
{
  "schemaVersion": 1,
  "id": "33333333-3333-4333-8333-333333333333",
  "type": "completeTodo",
  "createdAtUtc": "2026-08-21T12:10:00Z",
  "payload": {
    "todoId": 88
  }
}
```

## markWorkItemWorkedOn

The optional note is trimmed; blank text becomes `null`.

```json
{
  "schemaVersion": 1,
  "id": "44444444-4444-4444-8444-444444444444",
  "type": "markWorkItemWorkedOn",
  "createdAtUtc": "2026-08-21T12:15:00Z",
  "payload": {
    "workItemId": 4,
    "note": "Worked on IPv6 listener support"
  }
}
```

## setWorkItemPriority

```json
{
  "schemaVersion": 1,
  "id": "55555555-5555-4555-8555-555555555555",
  "type": "setWorkItemPriority",
  "createdAtUtc": "2026-08-21T12:20:00Z",
  "payload": {
    "workItemId": 4,
    "priority": "High"
  }
}
```

## setTodoPriority

```json
{
  "schemaVersion": 1,
  "id": "66666666-6666-4666-8666-666666666666",
  "type": "setTodoPriority",
  "createdAtUtc": "2026-08-21T12:25:00Z",
  "payload": {
    "todoId": 88,
    "priority": "Low"
  }
}
```

# Important Semantics

- `createWorkItem` with nested Todos is all-or-nothing. An invalid child rejects the whole command. Its applied receipt returns the WorkItem ID and all created Todo IDs.
- `completeTodo` is state-idempotent. It sets `Todo.CompletedAt`, updates the parent `WorkItem.LastWorkedAt`, and records WorkHistory.
- `markWorkItemWorkedOn` updates `LastWorkedAt` and records WorkHistory without completing a Todo.
- Set-priority commands may succeed as no-ops when the requested value is already set. Their receipts are applied, but planning state is unchanged.
- Reusing a processed command ID never reapplies its mutation, independently of whether the mutation itself is a no-op.

# Safety

- Never put tokens, API keys, credentials, or private operational secrets in command JSON.
- Never edit `state/snapshot.json` directly.
- Never invent unsupported command types.
- No destructive, delete, archive, lifecycle-reopen, or WorkItem-completion command is exposed through this bridge.
- Prefer explicit IDs from the latest snapshot over fuzzy name matching.

# Typical Assistant Workflow

## Add a Todo to HappyGopher

1. Fetch the snapshot and find HappyGopher's WorkItem ID.
2. Write one `createTodo` command with that ID.
3. Confirm the applied receipt or refreshed snapshot.

## Create a new project and its backlog

1. Write one `createWorkItem` command with nested Todos.
2. Wait for its applied receipt.
3. Use the returned WorkItem and Todo IDs for later commands.

## Mark a Todo done

1. Find the Todo ID in the latest snapshot.
2. Write one `completeTodo` command.

## I worked on this but did not finish anything

1. Find the WorkItem ID in the latest snapshot.
2. Write one `markWorkItemWorkedOn` command.

## Raise this project's priority

1. Find the WorkItem ID and current priority in the latest snapshot.
2. Write one `setWorkItemPriority` command.
