# Database installation

Each operation has a counterpart in both `English` and `Turkish`. The English schema uses `PersonnelManagementDB`. The Turkish schema uses `PersonelYonetimiDB` and Turkish object names. The application connects to the English schema; the Turkish version is an independent example.

| Operation | File in both folders |
| --- | --- |
| Schema only | `DatabaseSetup.sql` |
| Schema and synthetic demo records | `DatabaseSetupWithDemo.sql` |
| Add demo records to an installed, empty schema | `DatabaseDemoData.sql` |
| Validate personnel data and refresh report objects/indexes | `DatabaseUpdate.sql` |
| Copy a legacy Turkish database into the empty new schema | `DatabaseMigration.sql` |
| Create a limited SQL runtime account | `DatabaseApplicationAccount.sql` |
| Create the first administrator without demo records | `InitializeAdmin.ps1` |

Run only one setup alternative. Setup refuses to replace an existing database. Demo and migration scripts refuse to write into a populated target. Stop if a script reports an SQL error.

To migrate existing records, back up the source and close the application. Set `@LegacyDatabase` in the migration script (default `PersonelTakipV090Db`) to the actual source name. Run `DatabaseSetup.sql`, followed by `DatabaseMigration.sql`. The source database, record IDs and existing sign-in credentials are preserved. Existing V0.9 shift snapshots and cancellation history are copied. Missing plan fields are initialized using the current shift definition. New check-ins capture their own plan immediately.

Before running the update script, select the matching database in SSMS. It validates 11-digit personnel IDs and birth dates, adds their constraints and lookup/active-shift indexes, and refreshes report objects/covering indexes. Invalid existing records stop and roll back the update; no record is automatically corrected. Back up the database and close the application before running it; use migration to convert the old Turkish schema to English.

Demo credentials are `admin / Admin123!`, `manager / Manager123!`, `employee / Employee123!`. They do not replace accounts copied from the old database.

Use a separate application account with `SELECT`, `INSERT`, `UPDATE` on the application tables; `INSERT` on `AuditLog`; and `SELECT` on `EmployeeDetails`, `DailyAttendanceSummary`, `AnnualLeaveSummary` and `GetAbsences`. The account does not need `DELETE`, schema alteration, `sysadmin` or `db_owner` permissions.

Use `DatabaseApplicationAccount.sql` as a setup administrator to create a limited SQL login. Set its login/password variables locally before running. It refuses an existing login/user and does not reset passwords.

Use **Connection settings** on the sign-in screen to save the server, database and limited account credentials. Windows DPAPI protects the local file for the current Windows user. An environment connection override takes precedence over this file.

Covering indexes keep report fields in the index to reduce repeated table lookups. The update preserves the unique attendance record per employee and date. Re-running it does not rebuild indexes that already contain all required fields.

Application role codes: Admin (system administrator), Manager (HR manager), Employee (HR staff). Run DatabaseUpdate.sql on existing installations before assigning Manager accounts. Updating the schema does not create accounts or replace passwords. The Turkish example uses Admin, Yonetici and Personel. Both demo scripts create three synthetic accounts. See the root README for the permission matrix.

## First administrator without demo data

Build the application in Release and run `DatabaseSetup.sql` once. In **Windows PowerShell 5.1**, from the repository root:

```powershell
$sqlCredential = Get-Credential -Message 'SQL Server setup account'
$adminPassword = Read-Host 'Initial application administrator password' -AsSecureString
.\PersonnelManagement\Sources\English\InitializeAdmin.ps1 -Server 'SERVER\INSTANCE' -SqlCredential $sqlCredential -AdminPassword $adminPassword
```

The setup SQL login needs insert/select permissions on `UserAccount` and insert permission on `AuditLog`; it is separate from the application's Admin role and runtime SQL login. The password must contain 8–50 characters. The script uses SQL Server Authentication and encryption; add `-TrustServerCertificate` only for an explicitly trusted server with a self-signed certificate. It hashes the password with the application code, inserts the account and audit record together, and refuses to run if any account exists. It does not change connection settings. For the independent Turkish schema, use `Turkish/InitializeAdmin.ps1` after its matching schema setup.

Department/position screens are planned. For a new empty English schema, a setup administrator can create initial definitions in SSMS:

```sql
BEGIN TRANSACTION;
INSERT dbo.Department(DepartmentName) VALUES(N'Human Resources');
DECLARE @DepartmentId int=CONVERT(int,SCOPE_IDENTITY());
INSERT dbo.Position(DepartmentId,PositionName) VALUES(@DepartmentId,N'HR Specialist');
COMMIT TRANSACTION;
```

Create a shift in the application's shift screen, then add personnel. These example definitions are optional and must not be repeated against existing definitions. Keep SQL setup credentials outside the repository and configure a limited runtime SQL account for sign-in.
