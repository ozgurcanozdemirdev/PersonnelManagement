# PersonnelManagement 0.9.0

[Türkçe](README.tr.md)

A C# WinForms application for personnel records, shifts, attendance, leave requests and reporting. Version 0.9.0 improves attendance calculations, leave validation and report output. It includes user roles, database logging and regression tests.

## Features

Personnel records, departments and positions, shift management, attendance, leave requests and reports.

The UI is in Turkish. The root namespace and assembly name are `PersonnelManagement`.

## Requirements

- Windows with .NET Framework 4.8.1.
- Visual Studio or Build Tools with the .NET desktop workload and the 4.8.1 targeting pack.
- SQL Server 2022 or later, SSMS and SQL Server Authentication enabled. The local SQL login must have access to the selected database.
- Windows PowerShell 5.1 for the setup tools.

## Database and connection

1. Open SSMS and connect with **SQL Server Authentication**.
2. For a new installation, run `PersonnelManagement/Kaynak/DatabaseSetup.sql` in full. It creates `PersonelTakipV090Db` and stops if that database already exists. It does not delete existing data. Existing local installations do not need to rerun the setup script.
3. From this version's root folder, configure the application connection:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\ConfigureConnection.ps1 -Server "localhost\SQLEXPRESS" -Database "PersonelTakipV090Db"
```

Use your own SQL Server instance and SQL login. The password is requested securely; it is not stored in the source files. The connection is verified and then encrypted for the current Windows user at `%LOCALAPPDATA%\PersonnelManagement\V0.9.0\connection.dat`. This file cannot be transferred to another Windows user or computer; run the tool again there.

Encryption is enabled. When using a local development server with a self-signed certificate, add `-TrustServerCertificate`. Keep certificate validation enabled for servers with a trusted certificate.

The process environment variable `PERSONNELMANAGEMENT_V090_CONNECTION_STRING` overrides the saved local setting. Do not commit connection strings or put real passwords in README examples. `App.config`, `Settings.settings` and generated settings intentionally contain no default connection credentials.

The setup script contains example application accounts; their sample credentials are documented next to the seed statements. These are application login examples, not SQL Server credentials. Change them before using the application with actual personnel data. Application passwords use the original storage format.

## Build and run

Open `PersonnelManagement.slnx` in a Visual Studio version that supports this solution format. With other versions, open `PersonnelManagement/PersonnelManagement.csproj` directly. Build and run the application, or use a Developer PowerShell:

```powershell
MSBuild.exe .\PersonnelManagement\PersonnelManagement.csproj /t:Rebuild /p:Configuration=Release
```

No external NuGet package is required. Build output is generated locally and is ignored by Git.

## Tests

Regression checks are in [tests/README.tr.md](tests/README.tr.md). Set `PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING` only for the test process, using a local SQL Authentication account allowed to create and remove temporary test databases, then run:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tests\Verify.ps1
```

The tests use GUID-named temporary databases and remove them afterwards. They do not run against the normal personnel database. Physical printer output and every manual UI interaction are outside their scope.

## Version scope

The interface and existing identifiers are in Turkish. User login, role management and database logging are enabled. Application passwords use the original storage format.

See [CHANGELOG.md](CHANGELOG.md) for the changes included in this release.

## License

[MIT](LICENSE).
