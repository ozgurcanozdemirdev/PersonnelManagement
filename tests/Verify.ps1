param(
  [ValidateSet('Debug','Release')][string]$Configuration = 'Release',
  [string]$MSBuildPath
)
$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($env:PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING)) {
  throw 'Set PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING for a local SQL Server test instance. See tests/README.en.md or tests/README.tr.md.'
}
if ([string]::IsNullOrWhiteSpace($MSBuildPath)) {
  $command = Get-Command MSBuild.exe -ErrorAction SilentlyContinue
  if ($command) { $MSBuildPath = $command.Source }
  else {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path -LiteralPath $vswhere) {
      $MSBuildPath = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
    }
  }
}
if ([string]::IsNullOrWhiteSpace($MSBuildPath) -or -not (Test-Path -LiteralPath $MSBuildPath)) { throw 'MSBuild was not found.' }
$project = Join-Path $PSScriptRoot 'PersonnelManagement.Tests\PersonnelManagement.Tests.csproj'
& $MSBuildPath $project /t:Rebuild "/p:Configuration=$Configuration" /v:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw 'Test build failed.' }
$runner = Join-Path $PSScriptRoot "PersonnelManagement.Tests\bin\$Configuration\PersonnelManagement.Tests.exe"
$scripts = Join-Path (Split-Path $PSScriptRoot -Parent) 'PersonnelManagement\Sources'
& $runner $scripts
if ($LASTEXITCODE -ne 0) { throw 'Regression checks failed. Inspect TEST_ERROR and cleanup output.' }
