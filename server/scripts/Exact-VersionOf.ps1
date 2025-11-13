param([string]$PackageName = "dotnet")

$packageUrl = "https://api.nuget.org/v3-flatcontainer/$PackageName/index.json"

try {
    $data = Invoke-RestMethod -Uri $packageUrl -ErrorAction Stop
    $latest = $data.versions | Sort-Object { $_ -replace '[^0-9\.]', '' -as [version] } | Select-Object -Last 1
    Write-Output "Latest version of '$PackageName' is $latest"
}
catch {
    Write-Error "Package '$PackageName' not found."
}
