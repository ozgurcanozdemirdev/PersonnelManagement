#Requires -Version 5.1
[CmdletBinding()]
param(
  [string]$Server,
  [string]$Database = 'PersonnelManagementDB',
  [Alias('SqlCredential')][pscredential]$Credential,
  [string]$UserName,
  [securestring]$Password,
  [Alias('TrustLocalServerCertificate')][switch]$TrustServerCertificate,
  [string]$SettingsDirectory
)
& (Join-Path $PSScriptRoot '..\..\tools\ConfigureConnection.ps1') @PSBoundParameters
