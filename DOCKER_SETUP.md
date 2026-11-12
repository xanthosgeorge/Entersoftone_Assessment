# Docker setup — Entersoftone Assessment

## Prerequisites
- Docker Desktop (Linux containers / WSL2)
- At least 4 GB memory available to Docker for a smoother SQL Server startup

## Required application change
The current `Program.cs` only invokes `DatabaseSeeder.SeedAsync(context)` in Development.
A newly created container database has no tables until EF Core migrations run.

Inside the existing Development block in `Entersoftone_Assessment.Api/Program.cs`,
immediately before `await DatabaseSeeder.SeedAsync(context);`, add:

```csharp
await context.Database.MigrateAsync();
```

For transient SQL Server connection failures during first startup, enable retries
on the existing `UseSqlServer` registration:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));
```

These modifications are intended for local assessment/dev convenience.
Production schema migrations should normally run as a separate deployment step.

## Setup
1. Place `Dockerfile`, `compose.yaml`, `.dockerignore`, and `.env.example`
   at the repository root (beside `README.md`).
2. Copy `.env.example` to `.env`, and change the password:
   `Copy-Item .env.example .env` (PowerShell).
3. Ensure `.env` is listed in `.gitignore` (do not commit secrets).
4. Execute from repo root:

```powershell
docker compose up --build -d
docker compose ps
docker compose logs -f api
```

API Swagger UI (HTTP): http://localhost:8080/swagger

SQL Server available from the host at `localhost,14333` with user `sa`
and your chosen `.env` password. Inside Compose, API connects to `db,1433`.
No need for a SQL Server installation on the host.

## Useful commands
```powershell
docker compose down
docker compose down -v    # WARNING: destroys container DB volume
docker compose up --build -d
```

Change the `8080:8080` host mapping if port 8080 is occupied.

The database runs in the SQL Server 2022 Developer container.
The credentials are meant ONLY for local development and testing.

Note: If your particular SQL Server image does not include
`/opt/mssql-tools18/bin/sqlcmd`, adjust the healthcheck to the installed
sqlcmd location or use an alternative readiness probe.
