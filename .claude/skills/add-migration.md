# Add EF Core migration

Add a migration to the correct bounded context persistence project.

## Usage
`/add-migration <ContextName> <MigrationName>`

Example: `/add-migration UserAccounts AddRefreshTokenTable`

## Steps

1. Identify the persistence context project: `server/src/Hiscary.<ContextName>.Persistence.Context`
2. Run:
   ```
   dotnet ef migrations add <MigrationName> --project server/src/Hiscary.<ContextName>.Persistence.Context
   ```
3. Do NOT edit the generated migration files by hand.
4. Report the generated file paths.

## Valid context names
PlatformUsers, Stories, Media, Notifications, Recommendations, UserAccounts
