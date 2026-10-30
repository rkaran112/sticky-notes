# Builds a Release, framework-dependent, ReadyToRun single-file exe and installs it to
# %LocalAppData%\Programs\StickyNotesApp so it has a stable path for the autostart
# registry entry to point at (independent of where this repo happens to live).
$ErrorActionPreference = "Stop"

$dotnet = "$env:USERPROFILE\.dotnet\dotnet.exe"
if (-not (Test-Path $dotnet)) { $dotnet = "dotnet" }

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$publishDir = Join-Path $repoRoot "publish"
$installDir = "$env:LocalAppData\Programs\StickyNotesApp"

& $dotnet publish "$repoRoot\src\StickyNotes\StickyNotes.csproj" -c Release -o $publishDir
if ($LASTEXITCODE -ne 0) { throw "publish failed" }

New-Item -ItemType Directory -Force -Path $installDir | Out-Null
Copy-Item -Path "$publishDir\*" -Destination $installDir -Recurse -Force

Write-Host "Installed to $installDir\StickyNotes.exe"
Write-Host "(Autostart is registered automatically the first time the app runs.)"
