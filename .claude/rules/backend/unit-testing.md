---
paths:
  - "server/src/**/*.Tests/**/*.cs"
---

# Backend Unit Testing Conventions

- Stack: **xUnit v3** (`xunit.v3` package, not `xunit`) running under the **Microsoft Testing Platform** runner (`UseMicrosoftTestingPlatformRunner=true`), not VSTest. Test projects are named `{Ctx}.{Layer}.Tests` (e.g. `Hiscary.UserAccounts.Domain.Tests`), mirroring the project under test.
- Mocking: **NSubstitute** for all new tests (`Substitute.For<IInterface>()`). Do not introduce new `Moq` usage — Moq may still exist in older code but is not the pattern to follow going forward (see `backend/integration-testing.md` and NFR-03 of the config-plan spec for why: Moq's SponsorLink telemetry).
- Layout: strict **Arrange / Act / Assert**, with each section separated by a blank line and (optionally) a `// Arrange` / `// Act` / `// Assert` comment only when the boundaries aren't obvious from spacing alone.
- Naming: `MethodName_Scenario_ExpectedResult` for test method names (e.g. `AddComment_StoryNotFound_ReturnsClientSideError`). One behavior per test — don't assert multiple unrelated outcomes in a single `[Fact]`.
- Unit tests target `{Ctx}.Domain` and `{Ctx}.Application.*` types in isolation — no `DbContext`, no HTTP, no Aspire orchestration. Anything that needs a real Postgres/RabbitMQ/HTTP pipeline belongs in `{Ctx}.IntegrationTests` instead (see `backend/integration-testing.md`).
- Use `[Theory]` + `[InlineData]`/`[MemberData]` for parameterized cases rather than duplicating near-identical `[Fact]` methods.
- Reference `Microsoft.Testing.Extensions.CodeCoverage` for coverage collection (MTP-native, not `coverlet`).
