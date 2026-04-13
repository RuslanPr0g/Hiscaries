# Hiscary.UserAccounts.IntegrationTests

Containerized integration tests for `Hiscary.UserAccounts.Api.Rest` using Aspire AppHost testing.

## Local Run

Prerequisites:
- Docker daemon is running.
- .NET 10 SDK is installed.

Run:

```bash
dotnet test server/src/Hiscary.UserAccounts.IntegrationTests/Hiscary.UserAccounts.IntegrationTests.csproj --verbosity normal
```

## Architecture

- Uses `Aspire.Hosting.Testing` to boot `Hiscary.AppHost` and infrastructure containers.
- Reuses shared fixture/helpers from `Hiscary.Shared.IntegrationTesting` for future microservice modules.
- Creates service clients through Aspire resource names (for this suite: `hc-useraccounts-api-rest`).

## Conventions For Future Scenarios

- Keep shared fixtures/helpers in `Hiscary.Shared.IntegrationTesting`.
- Add endpoint/business-flow scenarios in `Scenarios/`.
- Name tests using `<Feature>_<Condition>_<Expected>`.
- Prefer one AppHost fixture per test suite and create per-resource clients as needed.

## Troubleshooting

- If tests fail before execution, verify Docker is available to the test process.
- If startup fails, verify AppHost resource names and Aspire references used by the fixture.
