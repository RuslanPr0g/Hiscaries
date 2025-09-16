$projectMappings = @{
    "../src/Hiscary.UserAccounts.Persistence.Context/Hiscary.UserAccounts.Persistence.Context.csproj" = "../src/Hiscary.UserAccounts.Api.Rest/Hiscary.UserAccounts.Api.Rest.csproj"
    "../src/Hiscary.Notifications.Persistence.Context/Hiscary.Notifications.Persistence.Context.csproj" = "../src/Hiscary.Notifications.Api.Rest/Hiscary.Notifications.Api.Rest.csproj"
    "../src/Hiscary.Stories.Persistence.Context/Hiscary.Stories.Persistence.Context.csproj" = "../src/Hiscary.Stories.Api.Rest/Hiscary.Stories.Api.Rest.csproj"
    "../src/Hiscary.PlatformUsers.Persistence.Context/Hiscary.PlatformUsers.Persistence.Context.csproj" = "../src/Hiscary.PlatformUsers.Api.Rest/Hiscary.PlatformUsers.Api.Rest.csproj"
}

Write-Host "Select a project to add a migration to:"
$index = 1
$projectMappings.Keys | ForEach-Object { Write-Host "$index. $_"; $index++ }
$choice = Read-Host "Enter the number (1-$($projectMappings.Count))"
$selectedProject = ($projectMappings.Keys | Select-Object -Index ($choice - 1))

if (-not $selectedProject) {
    Write-Host "Invalid selection. Exiting."
    exit
}

$startupProject = $projectMappings[$selectedProject]
$migrationName = Read-Host "Enter migration name"

Write-Host "Adding migration '$migrationName' to $selectedProject with startup project $startupProject..."
dotnet ef migrations add $migrationName `
    --project $selectedProject `
    --startup-project $startupProject `
    --output-dir Migrations

Write-Host "Migration '$migrationName' finished."
