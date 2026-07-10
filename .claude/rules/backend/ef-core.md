---
paths:
  - "server/src/*.Persistence.*/**/*.cs"
---

# EF Core Conventions

- Each bounded context owns one `*.Persistence.Context` project holding its `DbContext` (e.g. `Hiscary.Stories.Persistence.Context/StoriesContext.cs`), plus separate `*.Persistence.Read` / `*.Persistence.Write` projects for repositories. Don't put repository implementations in the `.Context` project or `DbContext` members in `.Read`/`.Write`.
- `DbContext` types derive from `BaseNucleusContext<TContext>` (from `StackNucleus.DDD.Persistence.EF.Postgres`), declare a `SCHEMA_NAME` constant, expose it via `SchemaName`, and apply entity configurations with `modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())` inside `OnModelCreating`. Always call `base.OnModelCreating(modelBuilder)` first.
- Entity configurations live under `Configurations/` as `IEntityTypeConfiguration<T>` implementations, one file per entity (e.g. `StoryConfigurations.cs`). Value objects that wrap a `Guid` id (e.g. `StoryId`) get a matching `*IdentityConverter.cs` for EF value conversion.
- Design-time factories derive from `NucleusDatabaseDesignTimeDbContextFactory<TContext>` (see `StoriesDatabaseDesignTimeDbContextFactory.cs`) — required for `dotnet ef` to resolve connection strings outside of Aspire orchestration.
- **Migrations are CLI-only, never hand-edited.** Generate with:
  ```
  dotnet ef migrations add {Name} --project server/src/Hiscary.{Ctx}.Persistence.Context
  ```
  Do not edit files under `Migrations/` directly, including `*ModelSnapshot.cs` — if a migration is wrong, add a new one or regenerate, don't patch the generated code.
- Schema application at startup uses pending-change semantics: each `*.Api.Rest/Program.cs` calls `context.Database.Migrate()` on boot, which applies any pending migrations and is a no-op if the schema is current. Don't call `EnsureCreated()` — it's incompatible with the migrations pipeline.
- Repository interfaces (`I{Entity}ReadRepository`, `I{Entity}WriteRepository`) live in `{Ctx}.Domain/DataAccess/` and extend `IBaseReadRepository<T, TId>` / `IBaseWriteRepository<T, TId>` from `StackNucleus.DDD.Domain.Repositories`. Implementations live in the matching `.Persistence.Read` / `.Persistence.Write` project — never reference `.Persistence.Context` types from `.Domain`.
- Read/Write split: query-only access (list/search/projection) goes through `.Persistence.Read` repositories returning read models from `{Ctx}.Domain/ReadModels/`; mutation goes through `.Persistence.Write` repositories operating on aggregate roots.
