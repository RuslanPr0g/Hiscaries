---
paths:
  - "server/src/**/*.IntegrationTests/**/*.cs"
  - "server/src/Hiscary.Shared.IntegrationTesting/**"
---

# Backend Integration Testing Conventions

- Pattern (verified against live code, not aspirational): tests use `AspireDistributedAppFixture<TEntryPoint>`, an abstract `IAsyncLifetime` in `Hiscary.Shared.IntegrationTesting/Aspire/AspireDistributedAppFixture.cs` that wraps `DistributedApplicationTestingBuilder.CreateAsync<TEntryPoint>(AppHostArgs)`, builds and starts the real Aspire app graph, and tears it down in `DisposeAsync`.
- Per bounded context, subclass the fixture against the real app host, e.g.:
  ```csharp
  public sealed class UserAccountsAppHostFixture : AspireDistributedAppFixture<Projects.Hiscary_AppHost>
  {
      public Task<HttpClient> CreateReadyUserAccountsClientAsync()
          => CreateHttpClientForResource("hc-useraccounts-api-rest");
  }
  ```
  Test classes consume it via `IClassFixture<TFixture>` constructor injection — see `Hiscary.UserAccounts.IntegrationTests/Scenarios/LoginUserTests.cs` as the reference example.
- `CreateHttpClientForResource(resourceName)` creates the client via `App.CreateHttpClient(resourceName)` and awaits `App.ResourceNotifications.WaitForResourceHealthyAsync(resourceName)` before returning — always go through this helper rather than constructing an `HttpClient` manually, so tests don't race the resource's startup.
- Use the `Hiscary.Shared.IntegrationTesting.Http` extensions (`PostJsonAsync`, `ReadRequiredJsonAsync`) and `Hiscary.Shared.IntegrationTesting.Assertions` (`AssertSuccess()`) helpers for HTTP calls and assertions instead of hand-rolling `HttpClient`/`JsonSerializer` calls.
- **State reset — proposed vs. current practice:** this repo does not yet reset the database between test runs. The current, actually-used workaround is generating unique per-test data (e.g. `$"integration_{Guid.NewGuid():N}"` for usernames, as in `LoginUserTests`) so tests don't collide on unique constraints. **Respawn is proposed** as a future replacement for this workaround but is **not yet adopted** — do not add a Respawn dependency or assert it as settled guidance until a spike confirms it's compatible with this repo's Aspire-managed Postgres container lifecycle (startup ordering after `.WaitFor()`, connection pooling). Until that spike lands, follow the unique-data-per-test pattern for any new integration test.
- Test projects are named `{Ctx}.IntegrationTests` and reference the real `{Ctx}.Api.Rest` entry point plus `Hiscary.AppHost` (via `Projects.Hiscary_AppHost`) — they exercise the full stack (API → Application → Domain → Persistence → Postgres), not a mocked subset.
