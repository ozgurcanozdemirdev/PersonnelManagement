# Regression checks

Run on Windows with Visual Studio MSBuild, the .NET Framework 4.8.1 targeting pack and a local SQL Server instance that accepts SQL Server Authentication. No external test framework is required.

The test administrator SQL login must be allowed to create/drop temporary databases and a temporary login. The application under test uses a separate temporary SQL login with table permissions, without `sysadmin` or `db_owner`. Use a development instance.

Set `PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING` in the current process using your local secret configuration. Do not save credentials in a source file or commit them. Then run from the repository root:

```powershell
.\tests\Verify.ps1 -Configuration Release
```

An optional `-MSBuildPath` selects MSBuild explicitly. The script builds the application and test runner. The runner creates only GUID-named `PersonnelManagementTest_...` and `PersonelYonetimiTest_...` databases, seeds synthetic records, verifies both schemas and repeatable updates, and removes temporary databases/login in `finally`. It does not modify the configured application database or its connection file.

Checks cover personnel validation, assigned shift deactivation, transactional audit failures, authentication rollback, role permissions, sync/async result equivalence and cancellation of an SQL read waiting on a lock. Performance output compares complete personnel details with the ID/name lookup on 10,000 added synthetic personnel, reporting five-run median time and SqlClient received bytes. Timing varies by machine; the assertion checks preserved choices and reduced data transfer, not a fixed speed ratio.

Success requires exit code 0, `CHECKS_PASSED=...`, `LIVE_DATABASE_WRITES=0` and `SCRATCH_RESOURCES_REMOVED=true`. Review `TEST_ERROR`, `CLEANUP_ERROR` or `LOGIN_CLEANUP_ERROR` after a failure. Log output contains test identifiers and measurements, not credentials.

First-administrator scripts are checked in both languages: atomic account/audit creation, hashed password verification and refusal when accounts exist. The concurrent shift test holds an assignment lock while deactivation starts, then confirms the shift remains active. SQL constraints also reject short IDs padded by `char(11)`.

Additional checks cover version-specific encrypted connection precedence and replacement, SQL Authentication configuration, immediate checkout, yearly leave balances, XLSX/CSV content, long-text print pagination, demo scripts in both languages, restricted SQL account permissions and legacy/V0.9 migration with preserved IDs, passwords and history. Printing tests render to a bitmap without sending jobs to a printer.

`CHECKS_PASSED` includes the role checks; `ROLE_CHECKS` reports that subset separately.
