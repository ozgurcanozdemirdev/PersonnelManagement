# PersonnelManagement

A C# WinForms application for human resources teams to manage personnel, shifts, attendance, leave requests and reports. Version **1.0.0** uses SQL Server, English code and database identifiers, and a Turkish/English interface.

[Türkçe açıklama](README.tr.md) · [Version history](CHANGELOG.md) · [MIT license](LICENSE)

## Features

- Personnel records with department, position and shift assignments.
- Check-in and checkout, overnight shifts, breaks and overtime calculations.
- A saved shift plan for each attendance entry, so later shift changes do not alter its calculation.
- Leave approval, overlap checks, cancellation history and annual balances split by year.
- Personnel, attendance, leave and absence reports with XLSX/CSV export and multi-page printing.
- Persistent TR/EN selection, including form captions and validation messages.
- Admin, HR Manager and HR Employee roles enforced in forms and data access.
- PBKDF2-SHA256 password hashing, encrypted local SQL settings and transactional audit records.
- Asynchronous personnel search/report reads, cancellation and compact ID/name lookups.

## Requirements

Windows, Visual Studio/MSBuild with WinForms support and the .NET Framework **4.8.1** targeting pack, and a local SQL Server instance that accepts **SQL Server Authentication**. The solution uses `.slnx`. No external NuGet package is required.

## Run with demo records

1. Open `PersonnelManagement.slnx` and build the application in Release.
2. In SQL Server Management Studio, run [English/DatabaseSetupWithDemo.sql](PersonnelManagement/Sources/English/DatabaseSetupWithDemo.sql). It creates `PersonnelManagementDB` and synthetic records; it refuses to overwrite an existing database. Run the whole file and stop on errors. If the name is already in use, choose a separate database name and replace its occurrences in the selected scripts before running them.
3. Run [English/DatabaseApplicationAccount.sql](PersonnelManagement/Sources/English/DatabaseApplicationAccount.sql) with a setup administrator. Choose a unique SQL login and supply a private password locally in the script's variables. Keep the password out of committed files. The new account has table/view permissions without `sysadmin`, `db_owner`, personnel deletion or audit updates.
4. Start `PersonnelManagement`. Open **Connection settings**, enter the server, database, SQL login and SQL password, then save. Select **Trust server certificate** only when you intend to trust that server's certificate. Encryption remains enabled.
5. Sign in with one of the synthetic application accounts below. Application accounts are separate from the SQL login.

| Account | Password | Role |
| --- | --- | --- |
| admin | Admin123! | Administrator |
| manager | Manager123! | HR Manager |
| employee | Employee123! | HR Employee |

Use `DatabaseSetup.sql` instead of the demo alternative for an empty installation. Create its first application administrator using `tools/CreateAdmin.ps1`; see [database instructions](PersonnelManagement/Sources/README.en.md).

## HR permissions

An Employee account represents a member of the HR department. It manages company personnel rather than a self-service employee portal.

| Operation | Admin | HR Manager | HR Employee |
| --- | --- | --- | --- |
| Dashboard and personnel records | Yes | Yes | Yes |
| Add/update personnel details and record attendance | Yes | Yes | Yes |
| Change annual allowance or end employment | Yes | Yes | No |
| View shifts and create leave requests | Yes | Yes | Yes |
| Maintain shifts; approve/reject/cancel leave | Yes | Yes | No |
| Reports, export and printing | Yes | Yes | No |
| Create accounts and assign roles | Yes | No | No |

The repository rechecks the current database role and active state before protected operations. The last active administrator cannot be demoted. HR staff preserve an existing annual allowance and use the default value for new personnel. This application does not calculate a statutory leave entitlement.

## Connection configuration

The application saves a Windows DPAPI file at `%LOCALAPPDATA%\PersonnelManagement\V1.0.0\English\connection.dat`. The separate profile avoids loading the earlier Turkish V1.0 schema's settings. If that profile does not exist, an encrypted `connection.dat` beside the executable is supported. A corrupt profile is reported instead of silently selecting another database.

The process variable `PERSONNELMANAGEMENT_V100_CONNECTION_STRING` takes precedence. No usable connection string or private SQL password is committed. The connection requires SQL Server Authentication and a limited SQL login; Windows Authentication and elevated runtime accounts are rejected.

Connection settings can also be configured from **Windows PowerShell 5.1**:

```powershell
$sqlCredential = Get-Credential -Message 'Limited application SQL login'
.\tools\ConfigureConnection.ps1 -Server 'SERVER\INSTANCE' -Credential $sqlCredential
```

Add `-TrustServerCertificate` when explicitly trusting a self-signed certificate. `-SettingsDirectory` can save an encrypted application-directory fallback. The tool checks the connection and English schema before replacing settings.

## Structure and validation

`UI` contains forms and localization; `DataAccess` contains parameterized SQL; `Models` contains data objects; `Security` handles passwords, validation and operation permissions; `Logging` writes transactional audits; `Reporting` exports reports.

Run `.\tests\Verify.ps1 -Configuration Release` with a process-only `PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING` for a local development instance. Tests create and remove GUID-named databases and SQL logins. See [test instructions](tests/README.en.md), [database scripts](PersonnelManagement/Sources/README.en.md) and [known limitations](docs/KnownLimitations.en.md).

The Turkish SQL copies have Turkish object names in a separate `PersonelYonetimiDB`. The application uses the English schema regardless of interface language. Migration scripts preserve old record IDs and credentials, saved V0.9 shift plans and cancellation history; records without a saved plan use the current shift definition. Existing live databases are not converted automatically.
