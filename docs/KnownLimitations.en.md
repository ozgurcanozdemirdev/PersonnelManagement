# Known limitations

The application serves HR staff. It does not provide employee self-service, statutory leave entitlement calculations, payroll, onboarding or offboarding workflows.

Department and position repositories exist, but dedicated maintenance screens are not included. Initial definitions can be inserted by a setup administrator; demo setup supplies examples. Password reset exists as an Admin-only repository operation. A password-reset screen, account activation screen and own-password screen are not included.

Legacy User roles are retained during migration for record history. An Admin must assign a supported Admin/Manager/Employee role before those accounts can sign in. Historical attendance without a saved shift plan is initialized from the current shift definition; the original historical definition cannot be reconstructed.

Async reads keep personnel search and report loading responsive and support cancellation. The personnel grid still loads the requested result set without server-side paging. The compact lookup query transfers less data; timings depend on the machine and data.

The Turkish schema is an independent SQL example. The application always uses the English schema. SQL login permissions and application roles serve different purposes. Direct access through the shared SQL login does not provide per-user HR authorization.

Tests cover synthetic databases and in-memory print rendering. They do not prove physical printer output, every display/DPI configuration or behavior against a production installation.
