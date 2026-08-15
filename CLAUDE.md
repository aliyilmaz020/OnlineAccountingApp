# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

Monorepo: the .NET backend lives in `api/` (contains `OnlineAccountingApp.slnx`); the React/Vite frontend lives in `ui/`. Run `dotnet` commands from `api/`; run `npm` commands from `ui/`.
Bu projede ayrı branch açılmayacak. master branchinden devam edilecek. Localde değişiklikler görülecek.
**Backend**

- Build: `dotnet build OnlineAccountingApp.slnx`
- Run the REST API: `dotnet run --project OnlineAccountingApp.WebApi` (http://localhost:5251, https://localhost:7025; Swagger UI at `/swagger` in Development)
- Run the gRPC host: `dotnet run --project OnlineAccountingApp.Grpc` (http://localhost:5158, https://localhost:7293/HTTP2; gRPC reflection on in Development)
- Run tests: `dotnet test OnlineAccountingApp.Application.Tests` (xUnit + Moq, covers Auth/Company/Role/MainRole/relationship/UniformChartOfAccount handlers)
- Run a single test: `dotnet test OnlineAccountingApp.Application.Tests --filter "FullyQualifiedName~LoginCommandHandlerTests.Handle_ShouldPassCredentialsThrough_AndReturnAuthServiceResult"` (or any substring of the fully-qualified test name, e.g. just the class name to run a whole fixture)
- Add an EF Core migration for the master DB: `dotnet ef migrations add <Name> --project OnlineAccountingApp.Persistence --startup-project OnlineAccountingApp.WebApi --context AppDbContext -o Migrations/AppDb`
- Add an EF Core migration for the per-company DB: `dotnet ef migrations add <Name> --project OnlineAccountingApp.Persistence --startup-project OnlineAccountingApp.WebApi --context CompanyDbContext -o Migrations/CompanyDb`
- Apply master DB migrations directly: `dotnet ef database update --project OnlineAccountingApp.Persistence --startup-project OnlineAccountingApp.WebApi --context AppDbContext`
- Apply per-company DB migrations: done at runtime via `GET /api/Companies/MigrateCompanyDb` (see below), not via `dotnet ef database update`, since each company has its own connection string.
- Seed sample data into the master DB (companies/users/roles/relationships) for local dev: `POST /api/Seed/SeedSampleData` (idempotent — safe to call repeatedly).

**Frontend** (from `ui/`)

- Install deps: `npm install`
- Dev server: `npm run dev` (Vite)
- Build: `npm run build` (`tsc -b && vite build`)
- Lint: `npm run lint`
- Env: copy `.env.example` to `.env` and set `VITE_API_BASE_URL` (defaults to `https://localhost:7025`, matching the WebApi's HTTPS port above)

Target framework is `net10.0` across all .NET projects.

## Architecture

### Backend (`api/`)

Clean Architecture layering across seven projects:

- **Domain** — POCOs and error codes only, no dependencies on other projects. `Abstracts/BaseEntity` (Id/CreateDate/EditDate/Status/Deleted) is the base for all entities. `Exceptions/AppErrorCodes.cs` defines the app-wide error code catalog. Entities split by which database they belong to:
  - `AppEntities/` — master-DB entities (`Company`, `UserCompany`, ASP.NET Identity's `AppUser`/`AppRole`), namespaced `OnlineAccountingApp.Domain.Entities` / `...Entities.Identity`.
  - `CompanyEntities/` — per-tenant-DB entities (e.g. `UniformChartOfAccount`), namespaced `OnlineAccountingApp.Domain.CompanyEntities`.
- **Framework** — dependency-free generic CQRS infrastructure reused by every CRUD feature: `Services/IRepository.cs`, `Services/IUnitOfWork.cs`, `Services/PagedResult.cs`, and template-method MediatR base classes under `MedatR/{Create,Update,Delete,GetById,GetList}/` (`Base*Command`/`Base*Query`, `Base*CommandHandler`/`Base*QueryHandler`, `Base*CommandValidator`). Each base handler's `Handle()` is sealed to a fixed flow (e.g. Create: check-exists → map → `BeforeCreateAsync` hook → persist inside a transaction → `AfterCreateAsync` hook → commit → map response); subclasses only override the `virtual` hook methods they need.
- **Application** — MediatR command/handler pairs under `Features/AppFeatures/<Feature>/<Action>/` (master DB) or `Features/CompanyFeatures/<Feature>/<Action>/` (tenant DB), service interfaces (`Services/AppServices/*`, `Services/CompanyServices/*`), and Mapster config (`Mapper/MapsterConfig.cs`). Depends on Domain and Framework only.
- **Persistence** — EF Core implementation: `Context/AppDbContext` (master DB, `IdentityDbContext<AppUser, AppRole, string>`) and `Context/CompanyDbContext` (per-tenant DB, built from a `Company`'s stored connection info). `Services/Repository<TEntity, TContext>` is the generic `IRepository<T>` implementation, generic over the EF Core context; `Services/UnitOfWork` wraps it for `AppDbContext` (`IUnitOfWork`) and `Services/CompanyServices/CompanyUnitOfWork` wraps it for `CompanyDbContext` (`ICompanyUnitOfWork`) — company-scoped handlers must inject `ICompanyUnitOfWork`, never `IUnitOfWork`, or their repository silently targets the master DB. `Services/AppServices/CompanyService` handles tenant-DB migration (`MigrateCompanyDbAsync`). Per-tenant entity configurations/table names live in `Configurations/` and `Constants/TableNames.cs`, applied only in `CompanyDbContext.OnModelCreating`. Depends on Application/Framework (and transitively Domain).
- **Infrastructure** — JWT support: `Services/JwtTokenService.cs` and `Options/JwtOptions.cs`. Depends on Application.
- **WebApi** — REST host. `Program.cs` wires up (in order) `ConfigureApi()` (Swagger + JWT scheme + `CompanyHeaderOperationFilter`), `AddPersistence()`, `AddConfigureAuthentication()` (must run after `AddPersistence()` — `AddIdentity` makes cookies the default scheme, so this call has to win), `AddInfrastructure()`, `AddApplication()` (MediatR + Mapster + service DI), via extension methods in `Configurations/` and `DependencyInjections/`. `Filters/ApiResultFilter.cs` wraps every controller response in the `ApiResponse` envelope; `ExceptionHandling/GlobalExceptionHandler.cs` catches `BusinessException`/`ValidationException` and maps them to `AppErrorCodes`. `Tenancy/RequiresCompanyHeaderAttribute.cs` marks controllers/actions that require the `X-Company-Id` header. Controllers derive from `Controllers/BaseApiController.cs` (implicitly `[Authorize]`; use `[AllowAnonymous]` to opt out, as `AuthController` does) and are thin MediatR dispatchers (`mediator.Send(command)`).
- **Grpc** — separate gRPC host (`OnlineAccountingApp.Grpc`) that dispatches the *same* MediatR commands/queries as the REST controllers, so business logic lives in one place across both protocols. It has its own `Program.cs`, ports, and `DependencyInjections/Grpc*DependencyInjection.cs` registrations — these are hand-synced copies of the WebApi's setup and do **not** reference the WebApi project, so a DI change on one side needs to be mirrored on the other. `.proto` contracts live in `Protos/`; only `Auth`, `Companies`, `Roles`, and `UniformChartOfAccounts` are exposed over gRPC so far (`MainRole*` features are REST-only). `Interceptors/BusinessExceptionInterceptor` is the gRPC equivalent of `GlobalExceptionHandler`, converting exceptions to `RpcException`s and carrying `AppErrorCodes` in trailing metadata (`error-code`, `errors-json`) since gRPC has no JSON body.
- **Application.Tests** — xUnit + Moq handler tests, referencing only Application.

#### Response envelope and error codes

Every REST response is wrapped as `{ success, data, errorCode, message, errors }` by `ApiResultFilter`. Error codes follow `{2-digit service code}{3-digit HTTP status}` (e.g. `02409` = UniformChartOfAccount service, 409 Conflict); the full table is in `AppErrorCodes` and mirrored in `README.md`. gRPC carries the same `AppErrorCodes` via trailing metadata instead of a JSON body, translating HTTP status to the nearest `Grpc.Core.StatusCode` (400→`InvalidArgument`, 401→`Unauthenticated`, 403→`PermissionDenied`, 404→`NotFound`, 409→`AlreadyExists`, unexpected→`Internal`).

### Multi-tenancy model

This is a multi-tenant accounting app: one master database (`AppDbContext`, connection string `SqlServer` in `appsettings.json`) holds `Company` records — each storing its own SQL Server connection info (`ServerName`, `DatabaseName`, `ServerUserId`, `ServerPassword`). Each company additionally gets its own database, modeled by `CompanyDbContext`, which is instantiated per-`Company` at runtime rather than injected via DI.

Flow: `POST /api/Companies/CreateCompany` creates a `Company` row in the master DB. `GET /api/Companies/MigrateCompanyDb` iterates every `Company` in the master DB, opens a `CompanyDbContext` against each one's own connection string, and applies pending EF Core migrations to that tenant database — this is how new per-tenant schema changes (under `Migrations/CompanyDb/`) get rolled out to all existing tenants.

A request states which company it belongs to via the `X-Company-Id` header (REST) or gRPC metadata; endpoints marked `[RequiresCompanyHeader]` validate that the token's user actually has a `UserCompany` row for that company before any tenant-DB work happens.

### Adding a feature (existing pattern)

Feature location depends on the target database: `Application/Features/AppFeatures/<Feature>/<Action>/` for the master DB, `Application/Features/CompanyFeatures/<Feature>/<Action>/` for a tenant DB.

- If the entity derives from `BaseEntity` (the common case — see `CompanyFeature/Create` or `UniformChartOfAccountFeature/Create` for reference), use the Framework base classes: a `Create/` folder holds `CreateXCommand : BaseCreateCommand<TResponse>`, `CreateXCommandHandler : BaseCreateCommandHandler<CreateXCommand, TEntity, TResponse>`, `CreateXCommandValidator : BaseCreateCommandValidator<CreateXCommand>` — same shape for Update/Delete/GetById/GetList via the matching `Base*` families. Override only the `virtual` hooks you need; don't touch the sealed `Handle()` flow. Company-DB handlers must take `ICompanyUnitOfWork` in the constructor (never `IUnitOfWork`), or `Repository<T>()` will silently resolve against the master DB.
- If the entity does *not* derive from `BaseEntity` (e.g. `AppRole`, which comes from ASP.NET Identity), the base classes don't apply — implement `IRequest<TResponse>` / `IRequestHandler<TCommand, TResponse>` / `AbstractValidator<TCommand>` directly, following `RoleFeature`.

Handlers are auto-registered by MediatR (`RegisterServicesFromAssembly`); validators are auto-registered via `AddValidatorsFromAssembly` and run through `ValidationBehavior`. Remaining steps: add Mapster mappings to `Application/Mapper/MapsterConfig.cs` (wired into a `Register...Mappings()` method called from `AddApplication()`); register any entity-specific service interface/implementation in `ApplicationDependencyInjection` (not needed when the Framework base classes cover everything); mark the controller `[RequiresCompanyHeader]` if it targets a tenant DB; add `[AllowAnonymous]` if the endpoint must be reachable without a token (controllers are `[Authorize]` by default via `BaseApiController`).

## graphify

This project has a knowledge graph at `graphify-out/` (root, spanning `api/` + `ui/`) and a narrower one at `api/graphify-out/` (backend only), each with god nodes, community structure, and cross-file relationships.

Rules:
- For codebase questions, first run `graphify query "<question>"` when a `graphify-out/graph.json` exists. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- If `graphify-out/wiki/index.md` exists, use it for broad navigation instead of raw source browsing.
- Read `graphify-out/GRAPH_REPORT.md` only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` from the repo root (or `graphify update .` from within `api/` for the backend-only graph) to keep the graph current (AST-only, no API cost).

## ui

Frontend is a TailAdmin React template (Vite + React 19 + TypeScript + Tailwind) wired up to the backend REST API:

- `src/lib/apiClient.ts` is the single integration point with the API: it stores/refreshes JWT access+refresh tokens and the selected company id in `localStorage`, attaches `Authorization` and `X-Company-Id` headers, unwraps the `ApiResponse<T>` envelope, and retries once on a 401 via silent token refresh (`setUnauthorizedHandler` lets `AuthContext` react to a failed refresh). `apiGet`/`apiPost`/`apiPut`/`apiDelete` are the only functions feature code should call.
- `src/context/AuthContext.tsx` and `src/context/CompanyContext.tsx` hold auth/session and selected-company state; `src/routes/ProtectedRoute.tsx` and `src/routes/RequireCompany.tsx` gate routes in `App.tsx` on those.
- `src/hooks/useCrud.ts` + `src/components/crud/CrudPage.tsx` are the generic list/create/update/delete building blocks that mirror the backend's Framework CRUD pattern — new entity pages should reuse these rather than hand-rolling fetch logic.
- Feature pages follow `src/pages/<Feature>/<Feature>ListPage.tsx` + `<Feature>Form.tsx`, registered as routes in `App.tsx` (e.g. `Companies`, `Roles`, `MainRoles`, `MainRoleAndRoleRelationships`, `MainRoleAndUserRelationships`, `UniformChartOfAccounts` — the last one wrapped in `RequireCompany` since it hits a tenant DB).
- i18n via `react-i18next`: `src/i18n/config.ts` initializes it with `tr`/`en` resources assembled from `src/locales/{tr,en}/<namespace>.json` (one JSON pair per feature/page, e.g. `companies.json`, `roles.json`, `crud.json`, `common.json`). Components pull their namespace with `useTranslation("<namespace>")` (or an array of namespaces) and call `t("key")` — never hardcode UI copy. Add new text to both the `tr` and `en` JSON files under the right namespace (creating a new namespace file pair + registering it in `i18n/config.ts` if none fits) instead of inlining a string.

Rules:
- Ui katmanındaki gereksiz componentler kaldırılacak.
- Ui katmanı Api katmanıyla entegre çalışacak.
- Tema rengi koyu olacak

