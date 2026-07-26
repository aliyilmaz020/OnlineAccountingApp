# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

The solution lives in the nested `OnlineAccountingApp/` folder (contains `OnlineAccountingApp.slnx`). Run all `dotnet` commands from there, e.g. `cd OnlineAccountingApp` first, or pass the solution/project path explicitly.

- Build: `dotnet build OnlineAccountingApp.slnx`
- Run the API: `dotnet run --project OnlineAccountingApp.WebApi` (http://localhost:5251, https://localhost:7025; Swagger UI at `/swagger` in Development)
- Add an EF Core migration for the master DB: `dotnet ef migrations add <Name> --project OnlineAccountingApp.Persistence --startup-project OnlineAccountingApp.WebApi --context AppDbContext -o Migrations/AppDb`
- Add an EF Core migration for the per-company DB: `dotnet ef migrations add <Name> --project OnlineAccountingApp.Persistence --startup-project OnlineAccountingApp.WebApi --context CompanyDbContext -o Migrations/CompanyDb`
- Apply master DB migrations directly: `dotnet ef database update --project OnlineAccountingApp.Persistence --startup-project OnlineAccountingApp.WebApi --context AppDbContext`
- Apply per-company DB migrations: done at runtime via the API (see below), not via `dotnet ef database update`, since each company has its own connection string.

There is no test project in the solution yet.

Target framework is `net10.0` across all projects.

## Architecture

Clean Architecture layering, four projects plus a currently-empty Infrastructure stub:

- **Domain** — POCOs only, no dependencies on other projects. `Abstracts/BaseEntity` (Id/CreateDate/EditDate/Status/Deleted) is the base for all entities. Entities are split by which database they belong to:
  - `AppEntities/` — master-DB entities (`Company`, `UserCompany`, ASP.NET Identity's `AppUser`/`AppRole`), namespaced `OnlineAccountingApp.Domain.Entities` / `...Entities.Identity`.
  - `CompanyEntities/` — per-tenant-DB entities (e.g. `UniformChartOfAccount`), namespaced `OnlineAccountingApp.Domain.CompanyEntities`.
- **Application** — MediatR command/handler pairs under `Features/AppFeatures/<Feature>/<Action>/`, service interfaces (`Services/AppServices/*`, `Services/IRepository.cs`), and Mapster config (`Mapper/MapsterConfig.cs`). Depends only on Domain.
- **Persistence** — EF Core implementation: `Context/AppDbContext` (master DB, `IdentityDbContext<AppUser, AppRole, string>`) and `Context/CompanyDbContext` (per-tenant DB, built from a `Company`'s stored connection info). `Services/Repository<T>` is the generic `IRepository<T>` implementation, always operating against `AppDbContext`; `Services/CompanyService` extends it for `Company`-specific queries and tenant-DB migration. Per-tenant entity configurations/table names live in `Configurations/` and `Constants/TableNames.cs`, applied only in `CompanyDbContext.OnModelCreating`. Depends on Application (and transitively Domain).
- **Infrastructure** — project stub with a reference to Application; no code yet.
- **WebApi** — ASP.NET Core host. `Program.cs` wires up `ConfigureApi()` (Swagger + JWT bearer scheme), `AddPersistence()` (EF Core + Identity registration), `AddApplication()` (MediatR + Mapster + service DI) via extension methods in `Configurations/` and `DependencyInjections/`. Controllers are thin MediatR dispatchers (`mediator.Send(command)`).

### Multi-tenancy model

This is a multi-tenant accounting app: one master database (`AppDbContext`, connection string `SqlServer` in `appsettings.json`) holds `Company` records — each storing its own SQL Server connection info (`ServerName`, `DatabaseName`, `ServerUserId`, `ServerPassword`). Each company additionally gets its own database, modeled by `CompanyDbContext`, which is instantiated per-`Company` at runtime rather than injected via DI (see `CompanyDbContext` constructor and `CompanyService.MigrateCompanyDbAsync`).

Flow: `POST /api/Companies/CreateCompany` creates a `Company` row in the master DB. `GET /api/Companies/MigrateCompanyDb` iterates every `Company` in the master DB, opens a `CompanyDbContext` against each one's own connection string, and applies pending EF Core migrations to that tenant database — this is how new per-tenant schema changes (under `Migrations/CompanyDb/`) get rolled out to all existing tenants.

### Adding a feature (existing pattern)

New Application use cases follow the CQRS-via-MediatR shape already established by `CompanyFeature`: a folder per feature under `Features/AppFeatures/<Feature>/<Action>/` containing an `IRequest<T>` command and its `IRequestHandler<TCommand, T>`, registered automatically via `AddMediatR(... RegisterServicesFromAssembly ...)`. Mapster type mappings for the feature are added in `Mapper/MapsterConfig.cs` and registered in `RegisterCompanyMappings()`-style methods called from `AddApplication()`.

## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- For codebase questions, first run `graphify query "<question>"` when graphify-out/graph.json exists. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- If graphify-out/wiki/index.md exists, use it for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).
