# What Should I Work On Today?

What Should I Work On Today? is a small, self-hosted planning tool for choosing useful work when time and energy are limited. It keeps higher-level work items, concrete todos, and recent work history together, then suggests active todos that fit the available energy and effort budget.

This is a personal, single-user application. It is not a multi-user SaaS product and does not include account registration, teams, roles, or tenant isolation.

## Features

- Create and edit work items with a name, kind, priority, description, and optional URL. Supported kinds are `Project`, `Maintenance`, `Learning`, `Idea`, and `ExternalIssue`.
- Add todos to work items and classify each todo by energy (`Low`, `Medium`, or `High`), effort (`Short`, `Medium`, or `Long`), and priority (`Low`, `Normal`, or `High`). Work-item and todo priority default to `Normal`.
- Ask the chooser for up to three active todos that fit the selected energy and effort limits.
- Request an unfiltered random active todo from the UI.
- Retrieve a deterministic daily pick through the API.
- Complete, reopen, edit, and delete todos. Completing a todo updates the work item's last-worked timestamp and records a history entry.
- Record general work with an optional note, review the 20 most recent history entries for a work item, and add or edit history notes.
- Complete/reopen, archive/unarchive, edit, or permanently delete work items. Completed and archived work items are excluded from chooser results.
- Search work items and todos from the home page, filter the work-item list, and search todos within a work item.
- Read and create data through an API, including bulk work-item and todo imports.
- Optionally mirror planning state and process versioned mutation commands through a separate private GitHub repository.
- Optionally publish successful and failed UI login telemetry to Mission Control.

### Chooser behavior

The filtered UI chooser considers incomplete todos on active work items whose energy and effort values are less than or equal to the selected limits. It ranks candidates using how long the parent work item has been neglected, capped at 365 days, plus random jitter and a small priority bias, and returns up to three results. Work items that have never been worked on receive the maximum age score.

Priority is deliberately simple: `Low` contributes -45 points, `Normal` contributes 0, and `High` contributes +45. The parent WorkItem and Todo contribute independently, so two `High` values add +90. Priority improves or reduces recommendation rank but does not hard-pin an item or override candidate eligibility and neglect in every case.

The UI's separate random action remains uniform by default: every active, incomplete Todo has the same chance, regardless of energy, effort, priority, neglect, or daily scoring. Enabling **Favor priority** switches to weighted random selection using only the WorkItem and Todo priorities. Their `Low`/`Normal`/`High` multipliers are combined, increasing the odds of higher-priority work without guaranteeing it; every candidate retains a non-zero chance.

`GET /api/daily-pick` uses the server's local calendar date and defaults to `Medium` energy and `Medium` effort. It replaces the UI chooser's random jitter with a value derived from the date, todo ID, and work-item ID, while applying the same parent and Todo priority biases. Given the same date and unchanged state, its ranking is repeatable. Data changes or an item reaching the 365-day age cap can change the pick.

## Architecture

| Area | Implementation |
| --- | --- |
| Runtime | .NET 10 / ASP.NET Core |
| UI | Blazor Interactive Server components |
| Login/logout | Razor Pages with ASP.NET Core cookie authentication |
| API | ASP.NET Core minimal APIs under `/api` |
| Persistence | Entity Framework Core 10 with SQLite |
| Optional sync | GitHub Contents API mailbox/state mirror |
| Telemetry | Optional `JoyfulReaperLib.MissionControl` client |
| Packaging | Multi-stage Docker build using .NET 10 SDK and ASP.NET runtime images |

The application uses `IDbContextFactory<AppDbContext>` for UI and API operations. The core data model contains work items, their todos, and work-history entries. A history entry can retain a todo task snapshot even if that todo is later edited or deleted.

## Authentication

The UI and API use separate authentication mechanisms:

- **UI:** one configured username and password are validated by the login Razor Page. A successful login creates a persistent, HTTP-only, secure cookie named `__Host-wsiwot-auth`. The cookie has a 30-day sliding lifetime.
- **API:** every route under `/api` requires the configured API key as a Bearer token: `Authorization: Bearer <key>`. The API key is compared using a fixed-time comparison.

The anonymous routes are `/login` and `GET /health/live`. The Blazor application requires authentication; logout is performed with an antiforgery-protected `POST /logout`.

### Mission Control login telemetry

When Mission Control is enabled, UI login attempts publish best-effort events:

| Event type | Data |
| --- | --- |
| `wsiwot.user.login.succeeded` | Username, UTC authentication time, remote IP |
| `wsiwot.user.login.failed` | Attempted username, UTC failure time, remote IP |

An event that is not accepted is logged as a warning. Mission Control is disabled by default and is not used for API-key authentication events.

## Configuration

ASP.NET Core configuration sources apply, including `appsettings.json`, user secrets in Development, and environment variables. These settings matter to the application:

| Key | Required | Default | Purpose |
| --- | --- | --- | --- |
| `Auth:Username` | Yes | Empty | Single UI username |
| `Auth:Password` | Yes | Empty | Single UI password |
| `Api:Key` | Yes | Empty/unset | Bearer key for all `/api` routes |
| `Database:Path` | No | `App_Data/WhatShouldIWorkOnToday.db` | Absolute path or path relative to the application content root |
| `MissionControl:Enabled` | No | `false` | Enables login event publishing |
| `MissionControl:BaseUrl` | No | `http://localhost:5190/` | Mission Control service URL |
| `MissionControl:ApiKey` | No | Empty | Mission Control credential |
| `MissionControl:TimeoutMilliseconds` | No | `1000` | Event publishing timeout |
| `GitHubSync:Enabled` | No | `false` | Enables the GitHub synchronization worker |
| `GitHubSync:Owner` | When sync enabled | Empty | Owner of the private sync repository |
| `GitHubSync:Repository` | When sync enabled | Empty | Private sync repository name |
| `GitHubSync:Branch` | When sync enabled | `main` | Branch containing state, commands, and receipts |
| `GitHubSync:Token` | When sync enabled | Empty/unset | GitHub token with Contents read/write permission on the sync repository |
| `GitHubSync:PollIntervalSeconds` | When sync enabled | `300` | Poll interval; must be at least 30 seconds |

The application validates `Auth:Username`, `Auth:Password`, and `Api:Key` at startup and will not start when any is blank. Do not commit real credentials to `appsettings.json`. For environment variables, replace `:` with `__`, for example `Auth__Username`, `Auth__Password`, and `Api__Key`.

## Local development

Prerequisites:

- .NET 10 SDK
- An HTTPS development certificate for the `https` launch profile

From the repository root, configure Development secrets:

```powershell
dotnet dev-certs https --trust
dotnet user-secrets set "Auth:Username" "your-username" --project .\WhatShouldIWorkOnToday\WhatShouldIWorkOnToday.csproj
dotnet user-secrets set "Auth:Password" "replace-with-a-password" --project .\WhatShouldIWorkOnToday\WhatShouldIWorkOnToday.csproj
dotnet user-secrets set "Api:Key" "replace-with-a-long-random-key" --project .\WhatShouldIWorkOnToday\WhatShouldIWorkOnToday.csproj
dotnet restore .\WhatShouldIWorkOnToday\WhatShouldIWorkOnToday.csproj --configfile .\NuGet.config
dotnet run --project .\WhatShouldIWorkOnToday\WhatShouldIWorkOnToday.csproj --launch-profile https
```

Open `https://localhost:7277`. The HTTPS launch profile also binds `http://localhost:5095`, but the authentication cookie is always secure, so HTTPS is the intended UI endpoint.

`NuGet.config` includes both NuGet.org and the repository's `local-nuget` directory, which contains the Mission Control client package used by the project.

## Database and migrations

SQLite is used for all application data. With the default setting, the database is created at `WhatShouldIWorkOnToday/App_Data/WhatShouldIWorkOnToday.db` when the project is run from the repository. The parent directory is created automatically.

At startup, the application calls EF Core's `MigrateAsync`, so a new database is created and all included migrations are applied automatically. The current migrations create the work-item and todo tables, move energy and effort classification to todos, add work history with editable notes, and add durable GitHub sync command receipts.

Persist and back up the SQLite database file in deployments. The Docker build context intentionally excludes `App_Data` and SQLite database files.

## API

All API routes require the Bearer API key.

| Method | Route | Behavior |
| --- | --- | --- |
| `GET` | `/api/work-items` | Lists work items with total and active todo counts |
| `GET` | `/api/work-items/{id}` | Gets one work item |
| `GET` | `/api/todos` | Lists incomplete todos by default |
| `GET` | `/api/todos/{id}` | Gets one todo |
| `GET` | `/api/daily-pick` | Gets the deterministic daily pick, or `204 No Content` when none qualifies |
| `GET` | `/api/random-pick` | Gets a uniform or optionally priority-weighted random active Todo, or `204 No Content` when none exists |
| `POST` | `/api/work-items` | Creates one work item |
| `POST` | `/api/work-items/{workItemId}/todos` | Creates one todo on an active work item |
| `POST` | `/api/todos/bulk` | Creates up to 100 todos across active work items |
| `POST` | `/api/work-items/bulk` | Creates up to 50 work items and up to 100 nested todos in one request |

`GET /api/todos` accepts `workItemId` and `includeCompleted` query parameters. For create requests, omitted or blank energy and effort values default to `Medium`; omitted or blank work-item kind defaults to `Project`; and omitted or blank WorkItem or Todo priority defaults to `Normal`. Priority accepts `Low`, `Normal`, or `High`, case-insensitively.

Set variables for the examples:

```powershell
$baseUri = "https://localhost:7277"
$apiKey = "replace-with-your-api-key"
```

Get the daily pick:

```powershell
curl.exe -H "Authorization: Bearer $apiKey" "$baseUri/api/daily-pick"
```

Get a uniform random pick (the default):

```powershell
curl.exe -H "Authorization: Bearer $apiKey" "$baseUri/api/random-pick"
```

Get a priority-weighted random pick:

```powershell
curl.exe -H "Authorization: Bearer $apiKey" "$baseUri/api/random-pick?favorPriority=true"
```

The optional `favorPriority` query parameter defaults to `false`. `false` selects uniformly; `true` uses the WorkItem and Todo priority-weighted random mode. `204 No Content` means there are no active, incomplete Todos.

Create a work item:

```powershell
curl.exe -X POST "$baseUri/api/work-items" -H "Authorization: Bearer $apiKey" -H "Content-Type: application/json" -d '{"name":"Documentation refresh","kind":"Maintenance","priority":"High","description":"Bring project docs up to date","url":"https://example.com/docs"}'
```

Create a todo:

```powershell
curl.exe -X POST "$baseUri/api/work-items/1/todos" -H "Authorization: Bearer $apiKey" -H "Content-Type: application/json" -d '{"task":"Write the deployment notes","energy":"Low","effort":"Short","priority":"Normal"}'
```

Bulk-create work items with nested todos:

```powershell
curl.exe -X POST "$baseUri/api/work-items/bulk" -H "Authorization: Bearer $apiKey" -H "Content-Type: application/json" -d '{"items":[{"name":"Documentation refresh","kind":"Maintenance","description":"Bring project docs up to date","todos":[{"task":"Review configuration","energy":"Low","effort":"Short"},{"task":"Rewrite README","energy":"Medium","effort":"Medium"}]}]}'
```

Invalid create requests return validation problems. Creating a todo for a completed or archived work item is rejected.

## GitHub Sync

GitHub sync is an optional bridge for tools that can read and write a private GitHub repository. SQLite remains the authoritative WSIWOT database. GitHub is only a read-only state mirror plus a mailbox for requested changes and their receipts; the SQLite database file is never uploaded or synchronized.

Use a separate **PRIVATE** repository for planning data, for example `wsiwot-sync`, with this layout:

```text
state/
  snapshot.json
commands/
  pending/
    <command-id>.json
  applied/
    <command-id>.json
  rejected/
    invalid-filename-<safe-hash>.json
```

The worker publishes `state/snapshot.json` only when a stable hash of meaningful WorkItems and Todos changes. The snapshot includes string priority values for every WorkItem and Todo. It processes pending commands in filename order, writes the result under `commands/applied/`, and only then deletes the pending file. A pending JSON file whose filename is not a valid command GUID is copied byte-for-byte to a deterministic, safely named file under `commands/rejected/` before the pending copy is removed. This quarantines unusable filenames for inspection instead of retrying them forever.

Supported version 1 commands are deliberately constrained:

| Command | Behavior |
| --- | --- |
| `createWorkItem` | Creates a WorkItem and, optionally, up to 100 initial Todos atomically |
| `createTodo` | Adds one Todo to an active WorkItem |
| `completeTodo` | Completes a Todo, updates its parent, and records work history |
| `markWorkItemWorkedOn` | Updates an active WorkItem's last-worked time and records an optional note without completing a Todo |
| `setWorkItemPriority` | Sets a WorkItem to `Low`, `Normal`, or `High` priority |
| `setTodoPriority` | Sets a Todo to `Low`, `Normal`, or `High` priority |

Delete, archive, WorkItem-completion, and reopen commands are not exposed. See [AGENTS.md](AGENTS.md) for the complete assistant operating procedure and command examples.

Create a WorkItem:

```json
{
  "schemaVersion": 1,
  "id": "4e98b35e-0be8-4d1c-a2f9-34757de40bd7",
  "type": "createWorkItem",
  "createdAtUtc": "2026-08-21T12:00:00Z",
  "payload": {
    "name": "Documentation refresh",
    "kind": "Maintenance",
    "priority": "High",
    "description": "Bring project docs up to date",
    "url": "https://example.com/docs",
    "todos": [
      {
        "task": "Review configuration",
        "energy": "Low",
        "effort": "Short",
        "priority": "Normal"
      }
    ]
  }
}
```

Create a Todo:

```json
{
  "schemaVersion": 1,
  "id": "90d6c881-0067-430a-a305-e8aa73ed95b7",
  "type": "createTodo",
  "createdAtUtc": "2026-08-21T12:05:00Z",
  "payload": {
    "workItemId": 12,
    "task": "Review configuration",
    "energy": "Low",
    "effort": "Short",
    "priority": "Normal"
  }
}
```

Complete a Todo:

```json
{
  "schemaVersion": 1,
  "id": "6d08bb7c-8455-4fd0-8116-124c1d05de2d",
  "type": "completeTodo",
  "createdAtUtc": "2026-08-21T12:10:00Z",
  "payload": {
    "todoId": 73
  }
}
```

The pending filename must use the same GUID as the JSON `id`, such as `commands/pending/4e98b35e-0be8-4d1c-a2f9-34757de40bd7.json`. Creation validation and defaults match the normal HTTP API. The optional `priority` field accepts `Low`, `Normal`, or `High`; old commands that omit it continue to create `Normal` priority records. Nested WorkItem creation is all-or-nothing and returns all created IDs. Priority changes that request the current value succeed without changing state. Completing a Todo still updates its parent WorkItem's last-worked timestamp and records work history, while `markWorkItemWorkedOn` records work without completing a Todo.

For durable idempotency, WSIWOT stores each command ID, status, processing time, and full receipt in SQLite in the same transaction as the requested mutation. If the process stops after the database commit but before the GitHub receipt is written, the next cycle recreates the missing receipt without applying the mutation again.

### GitHub sync setup

1. Create a separate private GitHub repository. Do not use the public WSIWOT source repository for personal planning data.
2. Create a fine-grained personal access token restricted to that repository with **Contents: Read and write** permission. GitHub also grants the metadata read access needed by the API. Avoid a broad classic `repo` token when a fine-grained token is available.
3. Configure the deployment through secrets or environment variables; never commit the token:

```powershell
$env:GitHubSync__Enabled = "true"
$env:GitHubSync__Owner = "your-github-owner"
$env:GitHubSync__Repository = "wsiwot-sync"
$env:GitHubSync__Branch = "main"
$env:GitHubSync__Token = "github_pat_replace-me"
$env:GitHubSync__PollIntervalSeconds = "300"
```

For Docker, pass the same values with `-e`, especially `-e GitHubSync__Token=...`. The repository does not need empty directories committed: WSIWOT creates snapshot, receipt, and quarantine paths through the GitHub API, while external tools create command files directly under `commands/pending/`.

When `GitHubSync:Enabled` is `false`, the worker exits immediately and makes no GitHub requests. GitHub sync adds no HTTP endpoint and does not change UI or API authentication.

## Docker

Build the image from the repository root:

```powershell
docker build -t what-should-i-work-on-today .
docker volume create wsiwot-data
```

For a local HTTP container, run in the Development environment and persist SQLite in the named volume:

```powershell
docker run --rm --name wsiwot -p 8080:8080 --mount source=wsiwot-data,target=/data -e ASPNETCORE_ENVIRONMENT=Development -e Auth__Username=your-username -e Auth__Password=replace-with-a-password -e Api__Key=replace-with-a-long-random-key -e Database__Path=/data/WhatShouldIWorkOnToday.db what-should-i-work-on-today
```

The image exposes port `8080`. For this local Development-mode container, open `http://localhost:8080`. Mission Control settings can be supplied with the same double-underscore environment-variable convention.

## Deployment notes

- Keep credentials outside the image and source tree.
- Store the configured SQLite path on persistent storage. The application is designed around one local SQLite database, not a distributed data store.
- Non-Development mode enables HSTS and HTTPS redirection for UI routes. Terminate and forward HTTPS so the application receives the correct request scheme; the secure UI cookie will not work over plain HTTP.
- `GET /health/live` is anonymous, returns `{ "status": "ok" }`, and is excluded from production HTTPS redirection.
- Run database backups appropriate for SQLite and the chosen storage platform.

## Current limitations

- Authentication is one configured UI user and one shared API key; there is no user or key management UI.
- The API currently supports reads and creation only. Updates, lifecycle changes, deletion, work-history access, and chooser filtering are UI-only.
- Search loads data into the Blazor page and filters in memory; there is no full-text index or pagination.
