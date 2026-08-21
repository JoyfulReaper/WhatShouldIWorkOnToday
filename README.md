# What Should I Work On Today?

What Should I Work On Today? is a small, self-hosted planning tool for choosing useful work when time and energy are limited. It keeps higher-level work items, concrete todos, and recent work history together, then suggests active todos that fit the available energy and effort budget.

This is a personal, single-user application. It is not a multi-user SaaS product and does not include account registration, teams, roles, or tenant isolation.

## Features

- Create and edit work items with a name, kind, description, and optional URL. Supported kinds are `Project`, `Maintenance`, `Learning`, `Idea`, and `ExternalIssue`.
- Add todos to work items and classify each todo by energy (`Low`, `Medium`, or `High`) and effort (`Short`, `Medium`, or `Long`).
- Ask the chooser for up to three active todos that fit the selected energy and effort limits.
- Request an unfiltered random active todo from the UI.
- Retrieve a deterministic daily pick through the API.
- Complete, reopen, edit, and delete todos. Completing a todo updates the work item's last-worked timestamp and records a history entry.
- Record general work with an optional note, review the 20 most recent history entries for a work item, and add or edit history notes.
- Complete/reopen, archive/unarchive, edit, or permanently delete work items. Completed and archived work items are excluded from chooser results.
- Search work items and todos from the home page, filter the work-item list, and search todos within a work item.
- Read and create data through an API, including bulk work-item and todo imports.
- Optionally publish successful and failed UI login telemetry to Mission Control.

### Chooser behavior

The filtered UI chooser considers incomplete todos on active work items whose energy and effort values are less than or equal to the selected limits. It ranks candidates using how long the parent work item has been neglected, capped at 365 days, plus random jitter, and returns up to three results. Work items that have never been worked on receive the maximum age score.

The UI's separate random action chooses uniformly from all active, incomplete todos and ignores the energy and effort filters.

`GET /api/daily-pick` uses the server's local calendar date and defaults to `Medium` energy and `Medium` effort. It replaces the UI chooser's random jitter with a value derived from the date, todo ID, and work-item ID. Given the same date, eligible records, and neglect-score ordering, its ranking is repeatable. Data changes or an item reaching the 365-day age cap can change the pick.

## Architecture

| Area | Implementation |
| --- | --- |
| Runtime | .NET 10 / ASP.NET Core |
| UI | Blazor Interactive Server components |
| Login/logout | Razor Pages with ASP.NET Core cookie authentication |
| API | ASP.NET Core minimal APIs under `/api` |
| Persistence | Entity Framework Core 10 with SQLite |
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

At startup, the application calls EF Core's `MigrateAsync`, so a new database is created and all included migrations are applied automatically. The current migrations create the work-item and todo tables, move energy and effort classification to todos, and add work history with editable notes.

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
| `POST` | `/api/work-items/{workItemId}/todos` | Creates one todo on an active work item |
| `POST` | `/api/todos/bulk` | Creates up to 100 todos across active work items |
| `POST` | `/api/work-items/bulk` | Creates up to 50 work items and up to 100 nested todos in one request |

`GET /api/todos` accepts `workItemId` and `includeCompleted` query parameters. For create requests, omitted or blank energy and effort values default to `Medium`; omitted or blank work-item kind defaults to `Project`.

Set variables for the examples:

```powershell
$baseUri = "https://localhost:7277"
$apiKey = "replace-with-your-api-key"
```

Get the daily pick:

```powershell
curl.exe -H "Authorization: Bearer $apiKey" "$baseUri/api/daily-pick"
```

Create a todo:

```powershell
curl.exe -X POST "$baseUri/api/work-items/1/todos" -H "Authorization: Bearer $apiKey" -H "Content-Type: application/json" -d '{"task":"Write the deployment notes","energy":"Low","effort":"Short"}'
```

Bulk-create work items with nested todos:

```powershell
curl.exe -X POST "$baseUri/api/work-items/bulk" -H "Authorization: Bearer $apiKey" -H "Content-Type: application/json" -d '{"items":[{"name":"Documentation refresh","kind":"Maintenance","description":"Bring project docs up to date","todos":[{"task":"Review configuration","energy":"Low","effort":"Short"},{"task":"Rewrite README","energy":"Medium","effort":"Medium"}]}]}'
```

Invalid create requests return validation problems. Creating a todo for a completed or archived work item is rejected.

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
- The repository currently has no automated test project.
