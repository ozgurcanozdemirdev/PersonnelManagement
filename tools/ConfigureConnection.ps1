#Requires -Version 5.1
param(
  [string]$Server,
  [string]$Database = 'PersonnelManagementDB',
  [Alias('SqlCredential')][pscredential]$Credential,
  [string]$UserName,
  [securestring]$Password,
  [Alias('TrustLocalServerCertificate')][switch]$TrustServerCertificate,
  [string]$SettingsDirectory = (Join-Path ([Environment]::GetFolderPath('LocalApplicationData')) 'PersonnelManagement\V1.0.0\English')
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data, System.Security
if ([string]::IsNullOrWhiteSpace($Server)) { $Server = Read-Host 'SQL Server' }
if (-not $Credential) {
  if ([string]::IsNullOrWhiteSpace($UserName)) { $UserName = Read-Host 'SQL login' }
  if (-not $Password) { $Password = Read-Host 'SQL password' -AsSecureString }
  $Credential = [pscredential]::new($UserName, $Password)
}
if ([string]::IsNullOrWhiteSpace($Server) -or [string]::IsNullOrWhiteSpace($Database) -or
    [string]::IsNullOrWhiteSpace($Credential.UserName) -or $Credential.UserName -ieq 'sa') {
  throw 'Server, database and a limited SQL login are required. The sa login cannot be used.'
}
$connection = $null
$plain = $null
$temporary = $null
try {
  $settings = [Data.SqlClient.SqlConnectionStringBuilder]::new()
  $settings['Data Source'] = $Server
  $settings['Initial Catalog'] = $Database
  $settings['Integrated Security'] = $false
  $settings['User ID'] = $Credential.UserName
  $settings['Password'] = $Credential.GetNetworkCredential().Password
  $settings['Encrypt'] = $true
  $settings['TrustServerCertificate'] = $TrustServerCertificate.IsPresent
  $settings['Connect Timeout'] = 5
  if ([string]::IsNullOrEmpty($settings['Password'])) { throw 'SQL password is required.' }
  $connection = [Data.SqlClient.SqlConnection]::new($settings.ConnectionString)
  # Bağlantı, İngilizce şema ve sınırlı yetki doğrulanmadan ayarı değiştirmeyiz.
  try {
    $connection.Open()
    $command = $connection.CreateCommand()
    try {
      $command.CommandText = "SELECT CASE WHEN IS_SRVROLEMEMBER('sysadmin')=1 OR IS_MEMBER('db_owner')=1 THEN 1 ELSE 0 END;"
      if ([int]$command.ExecuteScalar() -ne 0) { throw 'Elevated SQL account.' }
      $command.CommandText = 'SELECT TOP(0) EmployeeId FROM dbo.Employee; SELECT TOP(0) UserId FROM dbo.UserAccount;'
      $command.ExecuteNonQuery() | Out-Null
    } finally { $command.Dispose() }
  } catch { throw 'SQL verification failed. Check the server, English database schema, login, permissions and certificate settings.' }
  $plain = [Text.Encoding]::UTF8.GetBytes($settings.ConnectionString)
  $encrypted = [Security.Cryptography.ProtectedData]::Protect($plain, $null, [Security.Cryptography.DataProtectionScope]::CurrentUser)
  $directory = [IO.Path]::GetFullPath($SettingsDirectory)
  [IO.Directory]::CreateDirectory($directory) | Out-Null
  $destination = Join-Path $directory 'connection.dat'
  $temporary = Join-Path $directory ([Guid]::NewGuid().ToString('N') + '.tmp')
  [IO.File]::WriteAllBytes($temporary, $encrypted)
  if (Test-Path -LiteralPath $destination) { [IO.File]::Replace($temporary, $destination, [NullString]::Value) }
  else { [IO.File]::Move($temporary, $destination) }
  Write-Output 'SQL connection verified and encrypted settings saved for this Windows user.'
} finally {
  if ($connection) { $connection.Dispose() }
  if ($plain) { [Array]::Clear($plain, 0, $plain.Length) }
  if ($encrypted) { [Array]::Clear($encrypted, 0, $encrypted.Length) }
  if ($temporary -and (Test-Path -LiteralPath $temporary)) { Remove-Item -LiteralPath $temporary -Force }
  $settings = $null
}
