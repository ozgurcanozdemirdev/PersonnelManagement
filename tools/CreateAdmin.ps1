#Requires -Version 5.1
[CmdletBinding()]
param(
  [Parameter(Mandatory=$true)][string]$Server,
  [string]$Database = 'PersonnelManagementDB',
  [Parameter(Mandatory=$true)][pscredential]$SqlCredential,
  [Alias('UserName')][string]$AdminUsername = 'admin',
  [string]$FirstName = 'System',
  [string]$LastName = 'Administrator',
  [securestring]$AdminPassword,
  [switch]$TrustServerCertificate,
  [string]$AssemblyPath
)
# İlk admin hesabı İngilizce şemada oluşturulur; mevcut hesaplar korunur.
& (Join-Path $PSScriptRoot '..\PersonnelManagement\Sources\English\InitializeAdmin.ps1') @PSBoundParameters
