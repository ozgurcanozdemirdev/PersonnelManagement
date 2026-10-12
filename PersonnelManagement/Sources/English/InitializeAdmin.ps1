#Requires -Version 5.1
param(
  [Parameter(Mandatory=$true)][string]$Server,
  [string]$Database = 'PersonnelManagementDB',
  [Parameter(Mandatory=$true)][pscredential]$SqlCredential,
  [string]$AdminUsername = 'admin',
  [string]$FirstName = 'System',
  [string]$LastName = 'Administrator',
  [securestring]$AdminPassword,
  [switch]$TrustServerCertificate,
  [string]$AssemblyPath = (Join-Path $PSScriptRoot '..\..\bin\Release\PersonnelManagement.exe')
)
$ErrorActionPreference = 'Stop'
if (-not (Test-Path -LiteralPath $AssemblyPath)) { throw 'Build the application in Release before initializing the administrator.' }
foreach ($value in @($AdminUsername,$FirstName,$LastName)) {
  if ([string]::IsNullOrWhiteSpace($value) -or $value.Trim().Length -gt 50) { throw 'Username, first name and last name are required and must not exceed 50 characters.' }
}
if (-not $AdminPassword) { $AdminPassword = Read-Host 'Initial administrator password' -AsSecureString }
$pointer = [IntPtr]::Zero
$connection = $null
$transaction = $null
try {
  $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($AdminPassword)
  $plainPassword = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer)
  if ([string]::IsNullOrWhiteSpace($plainPassword) -or $plainPassword.Length -lt 8 -or $plainPassword.Length -gt 50) { throw 'Password must contain 8 to 50 characters.' }
  [Reflection.Assembly]::LoadFrom([IO.Path]::GetFullPath($AssemblyPath)) | Out-Null
  $hash = [PersonnelManagement.Security.PasswordHasher]::Hash($plainPassword)
  $plainPassword = $null
  $settings = [Data.SqlClient.SqlConnectionStringBuilder]::new()
  $settings['Data Source'] = $Server
  $settings['Initial Catalog'] = $Database
  $settings['Integrated Security'] = $false
  $settings['User ID'] = $SqlCredential.UserName
  $settings['Password'] = $SqlCredential.GetNetworkCredential().Password
  $settings['Encrypt'] = $true
  $settings['TrustServerCertificate'] = $TrustServerCertificate.IsPresent
  $settings['Connect Timeout'] = 10
  $connection = [Data.SqlClient.SqlConnection]::new($settings.ConnectionString)
  $connection.Open()
  # Account and audit creation are atomic; a populated account table is never overwritten.
  $transaction = $connection.BeginTransaction([Data.IsolationLevel]::Serializable)
  $command = $connection.CreateCommand()
  $command.Transaction = $transaction
  $command.CommandText = "IF EXISTS(SELECT 1 FROM dbo.UserAccount WITH (TABLOCKX,HOLDLOCK)) THROW 51000,N'Account table is not empty. Initial administrator setup was refused.',1;
    INSERT dbo.UserAccount(Username,PasswordHash,FirstName,LastName,Role,IsActive)
    VALUES(@Username,@Hash,@FirstName,@LastName,N'Admin',1);
    DECLARE @Id int=CONVERT(int,SCOPE_IDENTITY());
    INSERT dbo.AuditLog(UserId,Action,TableName,AffectedId,Description)
    VALUES(@Id,N'Bootstrap',N'UserAccount',@Id,N'Initial administrator created without demo records.');"
  $command.Parameters.Add('@Username',[Data.SqlDbType]::NVarChar,50).Value = $AdminUsername.Trim()
  $command.Parameters.Add('@Hash',[Data.SqlDbType]::NVarChar,100).Value = $hash
  $command.Parameters.Add('@FirstName',[Data.SqlDbType]::NVarChar,50).Value = $FirstName.Trim()
  $command.Parameters.Add('@LastName',[Data.SqlDbType]::NVarChar,50).Value = $LastName.Trim()
  try { $command.ExecuteNonQuery() | Out-Null } finally { $command.Dispose() }
  $transaction.Commit()
  Write-Output 'Initial administrator created. Store the selected password securely.'
}
catch {
  if ($transaction) { try { $transaction.Rollback() } catch {} }
  throw
}
finally {
  if ($transaction) { $transaction.Dispose() }
  if ($connection) { $connection.Dispose() }
  if ($pointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer) }
  $plainPassword = $null
  $hash = $null
  $settings = $null
}
