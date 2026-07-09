# Run tests

Run frontend or backend tests depending on context.

## Usage
`/test [scope]`

- `/test` — run all affected frontend tests
- `/test <filename>` — run a single spec file
- `/test integration` — run .NET integration tests

## Steps

**Frontend (Jest via Nx):**
- All: `cd client && nx affected:test --base=master`
- Single file: `cd client && nx test hiscaries-client --testFile=<path>`

**Backend (integration):**
- `dotnet test server/src/Hiscary.UserAccounts.IntegrationTests`
- For other contexts: `dotnet test server/src/Hiscary.<Ctx>.IntegrationTests` if the project exists

Report pass/fail counts and any failing test names.
