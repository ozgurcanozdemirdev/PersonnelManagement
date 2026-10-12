# Changelog

## 1.0.0

- Organize English classes and namespaces into UI, DataAccess, Models, Connection, Security, Logging and Reporting.
- Add persistent Turkish/English forms, role labels and validation messages.
- Define Admin, HR Manager and HR Employee permissions and recheck database roles before protected operations.
- Hash application passwords with PBKDF2-SHA256 and protect the last active administrator.
- Roll back business changes when transactional audit recording fails.
- Keep V0.9 attendance shift snapshots, overnight calculations, duplicate checks, leave balances and report/export/printing fixes.
- Preserve SQL datetime precision for immediate checkout and reject reversed query intervals in sync/async methods.
- Add asynchronous reads, cancellation and compact employee lookup queries.
- Use SQL Authentication, explicit certificate trust and a separate encrypted English-schema connection profile.
- Provide English/Turkish setup, demo, update, migration, limited SQL account and first-admin scripts.
- Preserve historical shift plans and cancellation records when migrating V0.9 databases.
- Extend regression coverage for roles, connections, migrations, demo schemas, SQL permissions, export and pagination.

## 0.9.0

- Improve attendance calculations, overnight checkout and duplicate-entry handling.
- Store the shift plan used when an attendance entry is created.
- Validate personnel assignments, leave dates, overlapping requests and annual leave allowance.
- Correct report calculations, XLSX/CSV export and print pagination.
- Preserve user login, role management and database logging.
- Match attendance timestamps to SQL datetime precision for immediate checkout.
- Add version-specific SQL Authentication settings and regression tests.
- Locate MSBuild automatically in the test runner.
- Include English and Turkish setup guides and an MIT license.
- Support updating encrypted connection settings in Windows PowerShell 5.1.

## 0.8.0

- Initial WinForms personnel, shift, attendance, leave and report management version.
