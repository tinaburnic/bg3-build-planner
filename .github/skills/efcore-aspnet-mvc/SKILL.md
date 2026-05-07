---
name: efcore-aspnet-mvc
description: 'Use when: configuring Entity Framework Core with SQLite or SQL Server in ASP.NET MVC, creating DbContext and models, adding migrations, seeding data, and writing LINQ queries with Include() for related data.'
argument-hint: 'Describe the EF Core task, target provider, and any entities or routes involved.'
user-invocable: true
disable-model-invocation: false
---

# EF Core for ASP.NET MVC

## When to Use
- Add or update EF Core providers (SQLite, SQL Server)
- Configure `DbContext` in Program.cs
- Create or update entity classes and relationships
- Generate migrations and update the database
- Seed initial data for development
- Replace mock repositories with real EF Core queries
- Add `Include()`/`ThenInclude()` for navigation properties

## Inputs to Ask For
- Target database provider (SQLite, SQL Server)
- Connection string name and value
- DbContext class name
- Whether to use migrations or EnsureCreated
- Seed data requirements (mock data or minimal sample)

## Procedure
1. **Verify packages**
   - Ensure EF Core base package is present.
   - Add provider package (SQLite or SQL Server).
   - Add `Microsoft.EntityFrameworkCore.Tools` and `Design` for migrations.
2. **Configure DbContext**
   - Add `DbContext` in Program.cs using the selected provider and connection string.
   - Ensure appsettings.json contains the matching connection string key.
3. **Model relationships**
   - Add keys and foreign keys.
   - Use required properties for non-null columns.
   - Add navigation properties and collections.
4. **Migrations**
   - Run `dotnet ef migrations add <Name>`.
   - Apply with `dotnet ef database update`.
5. **Seeding**
   - Use a `DbInitializer` that checks for existing data.
   - Insert seed data once and save changes.
6. **Queries**
   - Replace mock repositories with `_dbContext` queries.
   - Use `Include()`/`ThenInclude()` for related data used in views.

## Output Expectations
- DbContext configured with correct provider and connection string.
- Entities reflect intended relationships.
- Migrations build and apply cleanly.
- Controllers use EF Core LINQ queries.
- Seed data available for local development.

## Tips
- Keep `DbInitializer` idempotent (exit if data already exists).
- Prefer explicit Include() over lazy-loading for predictable queries.
- For many-to-many, EF Core will create join tables by convention unless explicit join entities are added.
