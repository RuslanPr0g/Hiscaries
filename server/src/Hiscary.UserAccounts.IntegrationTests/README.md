# Hiscary.UserAccounts.IntegrationTests

Containerized integration tests for `Hiscary.UserAccounts.Api.Rest` using Testcontainers.

## Local Run

Prerequisites:
- Docker daemon is running.
- .NET 10 SDK is installed.

Run:

```bash
dotnet test server/src/Hiscary.UserAccounts.IntegrationTests/Hiscary.UserAccounts.IntegrationTests.csproj --verbosity normal
```

## Conventions For Future Scenarios

- Keep infrastructure in `Fixtures/` and `Factories/`.
- Keep deterministic seed logic in `Seed/`.
- Add endpoint/business-flow scenarios in `Scenarios/`.
- Name tests using `<Feature>_<Condition>_<Expected>`.
- Prefer one container fixture per service test project.

## Troubleshooting

- If tests fail before execution, verify Docker is available to the test process.
- If startup fails, check injected settings in `UserAccountsWebApplicationFactory`.
