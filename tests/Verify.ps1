param(
  [ValidateSet('Debug','Release')][string]$Configuration = 'Release',
  [string]$MSBuildPath = ''
)
$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($env:PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING)) {
  throw 'PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING içinde yerel test sunucusunun SQL bağlantısını tanımlayın.'
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
if ([string]::IsNullOrWhiteSpace($MSBuildPath) -or -not (Test-Path -LiteralPath $MSBuildPath)) { throw 'MSBuild bulunamadı; MSBuildPath parametresini kullanın.' }
& $MSBuildPath (Join-Path $PSScriptRoot 'PersonnelManagement.Tests.csproj') /t:Rebuild "/p:Configuration=$Configuration" /nologo /v:minimal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
$testOutput = Join-Path $PSScriptRoot "bin\$Configuration\exports"
New-Item -ItemType Directory -Path $testOutput -Force | Out-Null
$setup = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\PersonnelManagement\Kaynak\DatabaseSetup.sql'))
& (Join-Path $PSScriptRoot "bin\$Configuration\PersonnelManagement.Tests.exe") 'unused' $setup $testOutput (Join-Path $PSScriptRoot 'LegacySchemaFixture.sql')
exit $LASTEXITCODE
