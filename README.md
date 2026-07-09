# 📚 Hiscaries Library of Stories

---

[C4 Diagram of the system](https://drive.google.com/file/d/1YVpVJS43djNFkMbAwFCGF1CzXhPL_sf-/view?usp=sharing)

---

## 🚀 Local Development Setup 🚀

### Prerequisites

You will need

- Docker Engine
- DotNet CLI and SDK

to run the application locally.

### Backend

To set up the backend environment, simply run:

```powershell
SetUp-LocalEnv.ps1
```

or bash if you're on linux

```bash
chmod +x setup-localenv.sh

setup-localenv.sh
```

This will configure the backend using .NET Aspire.

```powershell
Add-Migration.ps1
```

This will add a new migration to the chosen service.

To run the backend application, please set Hiscary.AppHost as your startup project and hit run!

#### Domain-Driven Design (DDD) with StackNucleus

This application leverages **StackNucleus**, a package that helps in modeling Domain-Driven Design (DDD) in .NET. StackNucleus is integrated into the backend to structure the domain logic and ensure clean, maintainable architectures. It simplifies the modeling of aggregates, entities, and value objects in a way that aligns with DDD principles.
See more here: https://github.com/RuslanPr0g/StackNucleus

#### Helpful!

List your user secrets:

```bash
chmod +x list-usersecrets.sh
list-usersecrets.sh
```

Build and run on linux (given you are within the /server directory):

```bash
dotnet restore

dotnet build

newgrp docker # optional

dotnet run --project ./Hiscary.AppHost/Hiscary.AppHost.csproj
```

### Frontend

Navigate to the frontend (client) directory, then run:

```bash
npm ci
npm run start
```

For full frontend setup, team conventions, and all commands, see
[`client/README.md`](client/README.md).

---

## AI-Assisted Development (Claude Code)

The repo is configured for [Claude Code](https://claude.ai/claude-code) with MCP servers, project skills, and an AI PR review workflow.

### Prerequisites

```bash
npm install -g @anthropic-ai/claude-code
```

### Environment Variables

Add these to your shell profile (`~/.zshrc` or `~/.bashrc`):

```bash
# GitHub MCP — Personal Access Token with `repo` scope
# Create one at: GitHub → Settings → Developer settings → Personal access tokens (classic)
export HISCARIES_GITHUB_TOKEN="ghp_your_token_here"

# Postgres MCP — local Aspire database connection string
export HISCARIES_POSTGRES_URL="postgresql://postgres:Hisc4ryDev!2025@localhost:5432/postgres"
```

Both are read by `.mcp.json` at the repo root. The MCP servers start automatically when you open Claude Code in this directory.

### Aspire DB Static Password (one-time)

The Postgres MCP connects to the Aspire-managed database. Set a fixed password via .NET User Secrets so it is stable across container restarts:

```bash
cd server/src/Hiscary.AppHost
dotnet user-secrets set "Parameters:postgres-password" "Hisc4ryDev!2025"
```

### Available Skills

Open Claude Code in this repo and use these slash commands:

| Skill | Example | What it does |
|-------|---------|-------------|
| `/spec` | `/spec issue:42` | Fetches GitHub issue, creates a `design.md` spec in `.claude/specs/` |
| `/implement` | `/implement issue:42` | Reads the spec, creates a branch, implements all tasks |
| `/ship` | `/ship` | Commits, pushes, opens a PR with auto-generated description and `ai` label |
| `/automation-ideas` | `/automation-ideas` | Suggests Claude Code automation opportunities for the codebase |
| `/security-audit` | `/security-audit` | Runs a security audit against the current changes |

### GitHub Actions — AI PR Review

Every PR to `master` is reviewed by Claude, which posts inline comments for defects only (critical / high / medium — no praise, no style notes).

**One-time setup for maintainers:**

1. **Secret** — repo → Settings → Secrets and variables → Actions → Secrets → New repository secret:
   - Name: `ANTHROPIC_API_KEY`
   - Value: your Anthropic API key from [console.anthropic.com](https://console.anthropic.com)
2. **Variable** — repo → Settings → Secrets and variables → Actions → Variables → New repository variable:
   - Name: `AI_REVIEW_ENABLED`
   - Value: `true`

To disable the AI review without a code change, set `AI_REVIEW_ENABLED = false` in the repo variable.

---

🎉 **Thank you for visiting the repository!**

Feel free to contribute or raise any issues to improve the application!
