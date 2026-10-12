param(
    [string]$Server,
    [string]$Database = 'PersonelTakipDb',
    [string]$UserName,
    [Security.SecureString]$Password,
    [switch]$TrustServerCertificate,
    [string]$SettingsDirectory = (Join-Path $env:LOCALAPPDATA 'PersonnelManagement\V0.8.0')
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data, System.Security
if ([string]::IsNullOrWhiteSpace($Server)) { $Server = Read-Host 'SQL Server' }
if ([string]::IsNullOrWhiteSpace($UserName)) { $UserName = Read-Host 'SQL login' }
if ($null -eq $Password) { $Password = Read-Host 'SQL password' -AsSecureString }
if ([string]::IsNullOrWhiteSpace($Server) -or [string]::IsNullOrWhiteSpace($Database) -or [string]::IsNullOrWhiteSpace($UserName)) {
    throw 'SQL server, database and login are required.'
}

$passwordPointer = [IntPtr]::Zero
$plainBytes = $null
$connection = $null
$temporaryFile = $null
try {
    $passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($Password)
    $builder = New-Object Data.SqlClient.SqlConnectionStringBuilder
    $builder['Data Source'] = $Server
    $builder['Initial Catalog'] = $Database
    $builder['Integrated Security'] = $false
    $builder['User ID'] = $UserName
    $builder['Password'] = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
    $builder['Encrypt'] = $true
    $builder['TrustServerCertificate'] = $TrustServerCertificate.IsPresent
    $builder['Connect Timeout'] = 5
    if ([string]::IsNullOrEmpty($builder.Password)) { throw 'SQL password is required.' }

    # Bağlantı doğrulanmadan mevcut yerel ayar değiştirilmez. SQL hesabı
    # oluşturulmaz ve veritabanında herhangi bir kayıt güncellenmez.
    $connection = New-Object Data.SqlClient.SqlConnection($builder.ConnectionString)
    try { $connection.Open() }
    catch { throw 'SQL connection failed. Check the server, database, login and certificate settings.' }
    $connection.Dispose()
    $connection = $null

    $plainBytes = [Text.Encoding]::UTF8.GetBytes($builder.ConnectionString)
    $protectedBytes = [Security.Cryptography.ProtectedData]::Protect($plainBytes, $null, [Security.Cryptography.DataProtectionScope]::CurrentUser)
    $directory = [IO.Path]::GetFullPath($SettingsDirectory)
    $null = New-Item -ItemType Directory -Path $directory -Force
    $destination = Join-Path $directory 'connection.dat'
    $temporaryFile = Join-Path $directory ([Guid]::NewGuid().ToString('N') + '.tmp')
    [IO.File]::WriteAllBytes($temporaryFile, $protectedBytes)
    if (Test-Path -LiteralPath $destination) { [IO.File]::Replace($temporaryFile, $destination, [NullString]::Value) }
    else { [IO.File]::Move($temporaryFile, $destination) }
    Write-Output 'SQL connection verified and saved for this Windows user.'
}
finally {
    if ($connection -ne $null) { $connection.Dispose() }
    if ($plainBytes -ne $null) { [Array]::Clear($plainBytes, 0, $plainBytes.Length) }
    if ($passwordPointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer) }
    if ($temporaryFile -and (Test-Path -LiteralPath $temporaryFile)) { Remove-Item -LiteralPath $temporaryFile -Force }
}
