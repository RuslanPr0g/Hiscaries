# Postmortem: CI-only integration test timeout (Aspire StartAsync)

- **Date:** 2026-07-10
- **Status:** Resolved
- **Impact:** `Hiscary.UserAccounts.IntegrationTests` failed on every CI run with `System.TimeoutException` after ~10 minutes, while passing locally. Blocked CI for backend changes for several hours across many diagnostic commits.

## Summary

The Aspire test fixture's `DistributedApplication.StartAsync` timed out in GitHub Actions only. Two independent root causes were stacked, so fixing either alone still failed. Both were environment differences between a developer machine and a fresh CI runner — the test code itself was correct.

## Root causes

1. **Empty JWT/salt configuration crashed every API at startup.**
   `JwtSettings__*` and `SaltSettings__StoredSalt` are supplied locally via .NET user secrets on `Hiscary.AppHost`. CI had none, so the AppHost forwarded empty strings and every API process died with an unhandled
   `Failed to convert configuration value '' at 'JwtSettings:TokenLifeTime'`
   seconds after launch — before Kestrel bound its port. Health probes reported `Connection refused` forever.

2. **The AppHost health probe rejected the untrusted dev certificate.**
   All APIs bind HTTPS (`WithHttpsEndpoint`). Once config was fixed, the resource health check (`WithHttpHealthCheck`) failed TLS validation with `AuthenticationException: UntrustedRoot`, keeping resources Unhealthy and the gateway `Waiting`, so `StartAsync` never completed.

## Fixes (all in `.github/workflows/be.build.yml`)

- Create the ASP.NET dev cert **and install it into the runner's system CA store**:
  `dotnet dev-certs https --export-path ... --format PEM` + `sudo update-ca-certificates`.
- Provide CI-only fallback values for `JwtSettings__Key/Issuer/Audience/TokenLifeTime` and `SaltSettings__StoredSalt` (must be a structurally valid bcrypt salt, e.g. `$2a$11$<22 bcrypt-alphabet chars>`), alongside the existing container-password parameters.

## Why it took so long to diagnose

Resource state/health transitions showed *that* the API processes exited (`Running → Finished`) but not *why*. An earlier attempt to stream app logs used the logical resource name — but Aspire logs project resources under **per-instance resource IDs** (name + suffix), so the stream was silently empty. Keying `ResourceLoggerService.WatchAsync(evt.ResourceId)` off resource notification events finally surfaced the startup exception, after which each fix was one commit.

## Lessons learned

- When an Aspire resource goes `Running → Finished`, get the **process's own console output** first; health-check telemetry only shows symptoms.
- `ResourceLoggerService.WatchAsync` must be keyed by `evt.ResourceId`, not the resource name.
- Anything sourced from user secrets is invisible CI state — the workflow must supply every value the AppHost forwards (`WithJwtAndSaltSettings` in `Program.cs` is the checklist).
- HTTPS-bound project resources need the dev cert to *exist* for Kestrel and to be *system-trusted* for the AppHost health probe.

## Troubleshooting quick reference

| Symptom | Likely cause | Check |
|---|---|---|
| `StartAsync` times out CI-only | API process crashing at startup | Watch app logs by `ResourceId` |
| Health probe: `Connection refused` | Process exited / never bound port | App logs for unhandled exception |
| Health probe: `UntrustedRoot` | Dev cert not in system CA store | `update-ca-certificates` step present? |
| `FormatException ... TimeSpan` at `JwtSettings:TokenLifeTime` | Missing user-secret equivalents in CI env | `JwtSettings__*` env vars in workflow |
| Elasticsearch container exits | `vm.max_map_count` too low / bind-mount owned by root | sysctl step + `chmod 777 elasticsearch-data` |
