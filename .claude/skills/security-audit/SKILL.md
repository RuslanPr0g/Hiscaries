---
name: security-audit
description: Run a full security audit of this repo — secrets/tokens exposed in the codebase and git history, OWASP Top 10 concerns across the .NET backend and Angular frontend, dependency vulnerabilities, and CI/CD misconfiguration — then create one GitHub issue per confirmed finding automatically. Invoke when the user says "security audit", "audit security", "check for exposed secrets", "run a security scan", or "/security-audit".
allowed-tools: Bash(git log *) Bash(git grep *) Grep Read Bash(npm audit *) Bash(dotnet list package --vulnerable *) mcp__github__create_issue mcp__github__list_issues
---

```!
echo "REPO: $(git remote get-url origin 2>/dev/null | sed 's|.*github.com[:/]||;s|\.git$||')"
```

## Hard safety rule — read this first

**Never paste an actual discovered secret's plaintext value into chat output or an issue body.** This repo is public — an issue containing a live token is itself a leak. Redact to the first 4–6 characters plus `***` (e.g. `ghp_KxzT***`), mark the finding **Critical**, and lead the remediation with "rotate this credential immediately," not just "remove it."

---

## Step 1 — Run the checks

Work through all four categories below. Only report **confirmed** findings — something you actually located at a specific path/line or actually ran and observed — not speculative "you should probably check X" items.

### A. Secrets & credentials exposed to git

- Grep the **working tree** for key/token/password/connection-string patterns in `appsettings*.json`, `launchSettings.json`, docker/compose files, `.env*`, and any `*.config` files: patterns like `password\s*=`, `ConnectionString`, `api[_-]?key`, `secret`, `ghp_`, `github_pat_`, `AKIA[0-9A-Z]{16}`, `-----BEGIN.*PRIVATE KEY-----`.
- Separately search **git history**, not just the current tree — a secret removed in a later commit still lives in history: `git log -p --all -S'<pattern>' -- <suspicious-path>` or `git log --all --oneline -- '**/appsettings*.json'` followed by inspecting diffs of past versions.
- Check `.gitignore` actually covers `.env*`, `*.pfx`, `*.pem`, and any `appsettings.*.json` overrides beyond the base file.
- Check GitHub Actions workflows (`.github/workflows/*.yml`) for secrets echoed to logs (e.g. `run: echo ${{ secrets.X }}`) or unsafe `pull_request_target` combined with checkout of untrusted PR code.

### B. OWASP Top 10 (2021), mapped to this stack

- **A01 Broken Access Control** — API controllers missing `[Authorize]`; JWT validation config in `Hiscary.Shared.Jwt`; CORS policy in the AppHost/gateway.
- **A02 Cryptographic Failures** — hardcoded salts/keys committed to config (e.g. a static `StoredSalt` in `appsettings.json` instead of a per-user salt or secret-manager value), weak hashing, missing TLS enforcement.
- **A03 Injection** — raw SQL or string-concatenated EF Core queries; unsanitized `[innerHTML]` bindings or `bypassSecurityTrust*` calls in Angular templates without a sanitization step.
- **A04 Insecure Design** — missing rate limiting on auth/login endpoints; predictable password-reset flow.
- **A05 Security Misconfiguration** — wildcard (`*`) CORS origins, Swagger/OpenAPI exposed without gating in production, default/blank infra credentials in `Hiscary.AppHost/Program.cs`.
- **A06 Vulnerable & Outdated Components** — see dependency scan (C) below.
- **A07 Identification & Authentication Failures** — weak password policy, missing refresh-token rotation/revocation, long-lived or non-expiring JWTs.
- **A08 Software & Data Integrity Failures** — unpinned third-party GitHub Actions (using `@main`/`@master` instead of a version tag or SHA), risky `postinstall`/build scripts.
- **A09 Security Logging & Monitoring Failures** — auth failures and privilege-relevant actions not logged anywhere observable.
- **A10 SSRF** — any server-side code that fetches a URL derived from user input (check `Hiscary.Media.DocumentTools.PuppeteerSharp` and similar document-rendering/fetch paths).

### C. Dependency vulnerabilities

- `cd client && npm audit --json` — parse for `high`/`critical` advisories.
- `cd server/src && dotnet list Hiscary.sln package --vulnerable --include-transitive` — parse for reported vulnerable packages.

### D. CI/CD

- Workflow permission scopes wider than needed (e.g. `permissions: write-all`).
- Third-party actions pinned to a mutable ref instead of a tag/SHA.
- Secrets available to workflows triggered by `pull_request` from forks.

---

## Step 2 — Few-shot calibration (finding format, not exhaustive)

Use these as the bar for specificity and severity — findings should read like this, anchored to real paths, not generic advice:

> **Title:** Hardcoded bcrypt salt committed in `appsettings.json`
> **Severity:** Medium
> **OWASP:** A02 — Cryptographic Failures
> **Evidence:** `server/src/appsettings.json` — `SaltSettings.StoredSalt` is a static value committed to source control and shared across all environments.
> **Remediation:** Move to a per-environment secret (user-secrets locally, a secret manager or Aspire parameter in deployed environments) and rotate the current value since it has been exposed via version control.

> **Title:** Unsanitized `[innerHTML]` binding in story reader
> **Severity:** High
> **OWASP:** A03 — Injection
> **Evidence:** `client/apps/hiscaries-client/src/app/stories/read-story-content/read-story-content.component.html:168` binds `[innerHTML]="currentPageContent"` directly; confirm whether `currentPageContent` is sanitized upstream (e.g. via Angular's `DomSanitizer`) or could contain user/author-supplied HTML.
> **Remediation:** Sanitize with Angular's built-in sanitizer or a vetted HTML-sanitization library before binding; add a regression test asserting script tags are stripped.

> **Title:** Live GitHub token committed to a tracked file
> **Severity:** Critical
> **OWASP:** N/A — Secrets exposure
> **Evidence:** `<path>:<line>` — a token matching `ghp_***` (redacted) is present in a tracked file / git history.
> **Remediation:** Rotate the token immediately at github.com/settings/tokens, then remove it from history (`git filter-repo` or BFG) since a simple new commit does not remove it from history.

---

## Step 3 — De-dupe against previous runs

Call `mcp__github__list_issues` with `labels: ["security-audit"]`, `state: "all"`. Skip creating an issue whose title matches (or near-matches) one already present, open or closed.

---

## Step 4 — Create one issue per confirmed finding

For each confirmed, non-duplicate finding, call `mcp__github__create_issue`:

- `title`: short and specific (as in Step 2's examples)
- `body`:
  ```markdown
  ## Finding

  [What was found and why it matters, in this repo's context.]

  ## Evidence

  [Exact path:line. If a secret value, redact per the hard safety rule above.]

  ## OWASP Category

  [e.g. A03 — Injection, or "N/A — Secrets exposure"]

  ## Severity

  [Critical | High | Medium | Low]

  ## Remediation

  [Concrete next step — name the actual fix, not "review this."]
  ```
- `labels`: `["security-audit", "severity-<critical|high|medium|low>"]`

This runs automatically as part of invoking this skill — do not pause to ask the user which findings to file (unlike `automation-ideas`, every confirmed finding here gets an issue).

---

## Step 5 — Report

List each created issue: title, severity, URL. Do not restate evidence or secret values in the summary — link to the issue instead.
