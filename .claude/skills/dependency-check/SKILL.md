---
name: dependency-check
description: Report outdated and vulnerable packages across the backend (Directory.Packages.props / Hiscary.sln) and frontend (client/package.json) in one combined summary. Invoke when the user says "dependency check", "check for outdated packages", "any vulnerable dependencies", or "/dependency-check".
allowed-tools: Bash Read
---

## Usage

`/dependency-check`

## Backend

`dotnet list package --outdated` and `--vulnerable` **cannot be combined in one call** — run them separately:

```
dotnet list server/src/Hiscary.sln package --outdated
dotnet list server/src/Hiscary.sln package --vulnerable
```

Both commands emit `NU1902`/`NU1903` warnings inline during restore even without `--vulnerable` — don't mistake those restore-time warnings for the `--vulnerable` report itself; run the explicit command too so the report is complete and not just whatever happened to print during restore.

## Frontend

```
cd client
npm outdated
npm audit --json
```

Summarize `npm audit`'s JSON output by severity count (critical/high/moderate/low) rather than dumping the raw JSON — it's large. `npm outdated` output is already compact; include it close to verbatim.

## Output format

Two sections, **Backend** and **Frontend**, each with two subsections (Outdated, Vulnerable). For vulnerabilities, always name the specific package, current version, and severity — a summary like "3 vulnerabilities found" without naming them isn't actionable. Cross-reference: if the same underlying library shows up outdated in both `--outdated` and `--vulnerable` (common — an outdated package is often also the vulnerable one), note that explicitly since upgrading once fixes both findings.

Do not attempt to upgrade anything automatically — this skill reports, it doesn't modify `Directory.Packages.props`, `package.json`, or lock files. If the user wants to act on a finding, that's a separate, explicit follow-up.
