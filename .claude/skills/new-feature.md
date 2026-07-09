# Scaffold a new feature

Create a feature branch and stub out the required files for a new feature.

## Usage
`/new-feature <feature-name>`

## Steps

1. Create branch: `git checkout -b feature/<feature-name>`
2. Ask the user: frontend only, backend only, or both?
3. **Frontend** (Angular): create a standalone component under the appropriate domain folder in `client/apps/hiscaries-client/src/app/<domain>/`. Use path alias imports (`@shared/*`, etc.).
4. **Backend** (per bounded context): stub command/query in `Application.Write` or `Application.Read`, handler, and register in the relevant `Api.Rest` controller.
5. Run `nx lint hiscaries-client` after any frontend changes.
6. Report created files and branch name.
