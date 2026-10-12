#Requires -Version 5.1
param(
  [Parameter(Mandatory=$true)][string]$Server,
  [string]$Database = 'PersonelYonetimiDB',
  [Parameter(Mandatory=$true)][pscredential]$SqlCredential,
  [string]$AdminUsername = 'admin',
  [string]$FirstName = 'System',
  [string]$LastName = 'Administrator',
  [securestring]$AdminPassword,
  [switch]$TrustServerCertificate,
  [string]$AssemblyPath = (Join-Path $PSScriptRoot '..\..\bin\Release\PersonnelManagement.exe')
)
$ErrorActionPreference = 'Stop'
if (-not (Test-Path -LiteralPath $AssemblyPath)) { throw 'İlk yöneticiyi oluşturmadan önce uygulamayı Release olarak derleyin.' }
foreach ($value in @($AdminUsername,$FirstName,$LastName)) {
  if ([string]::IsNullOrWhiteSpace($value) -or $value.Trim().Length -gt 50) { throw 'Kullanıcı adı, ad ve soyad zorunludur; en fazla 50 karakter olabilir.' }
}
if (-not $AdminPassword) { $AdminPassword = Read-Host 'İlk yönetici şifresi' -AsSecureString }
$pointer = [IntPtr]::Zero
$connection = $null
$transaction = $null
try {
  $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($AdminPassword)
  $plainPassword = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer)
  if ([string]::IsNullOrWhiteSpace($plainPassword) -or $plainPassword.Length -lt 8 -or $plainPassword.Length -gt 50) { throw 'Şifre 8 ile 50 karakter arasında olmalıdır.' }
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
  # Hesap ve denetim kaydı birlikte oluşturulur; dolu kullanıcı tablosunun üzerine yazılmaz.
  $transaction = $connection.BeginTransaction([Data.IsolationLevel]::Serializable)
  $command = $connection.CreateCommand()
  $command.Transaction = $transaction
  $command.CommandText = "IF EXISTS(SELECT 1 FROM dbo.Kullanici WITH (TABLOCKX,HOLDLOCK)) THROW 51000,N'Kullanıcı tablosu boş değil. İlk yönetici kurulumu reddedildi.',1;
    INSERT dbo.Kullanici(KullaniciAdi,Sifre,Ad,Soyad,Rol,AktifMi)
    VALUES(@Username,@Hash,@FirstName,@LastName,N'Admin',1);
    DECLARE @Id int=CONVERT(int,SCOPE_IDENTITY());
    INSERT dbo.SistemLog(KullaniciId,Islem,TabloAdi,EtkilenenId,Aciklama)
    VALUES(@Id,N'Bootstrap',N'Kullanici',@Id,N'Demo verisi yüklemeden ilk yönetici oluşturuldu.');"
  $command.Parameters.Add('@Username',[Data.SqlDbType]::NVarChar,50).Value = $AdminUsername.Trim()
  $command.Parameters.Add('@Hash',[Data.SqlDbType]::NVarChar,100).Value = $hash
  $command.Parameters.Add('@FirstName',[Data.SqlDbType]::NVarChar,50).Value = $FirstName.Trim()
  $command.Parameters.Add('@LastName',[Data.SqlDbType]::NVarChar,50).Value = $LastName.Trim()
  try { $command.ExecuteNonQuery() | Out-Null } finally { $command.Dispose() }
  $transaction.Commit()
  Write-Output 'İlk yönetici oluşturuldu. Seçtiğiniz şifreyi güvenli şekilde saklayın.'
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
