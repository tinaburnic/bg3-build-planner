# BG3BuildPlanner

## Regenerate Database (SQLite)

This project uses SQLite at `BG3BuildPlanner/bg3buildplanner.db` (from `appsettings.json`).

1. Stop the app if it is running.
2. Delete the database files:
	- `BG3BuildPlanner/bg3buildplanner.db`
	- `BG3BuildPlanner/bg3buildplanner.db-shm`
	- `BG3BuildPlanner/bg3buildplanner.db-wal`
3. Run the app again. `DbInitializer` will reseed the database.