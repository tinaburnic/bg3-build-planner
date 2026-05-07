---
name: list-pages
description: 'Use when: creating a new list page in ASP.NET MVC (index/list views), including controller actions, view models, and Razor table or card layouts.'
argument-hint: 'Provide the entity name, list fields, filters/sort needs, and route expectations.'
user-invocable: true
disable-model-invocation: false
---

# List Pages (Index Views)

## When to Use
- Create a new list/index page for an entity
- Add filtering, sorting, or pagination
- Display related data in a list (Include/ThenInclude)
- Add semantic routes for list pages

## Inputs to Ask For
- Entity name and primary list fields
- Related data to show (navigation properties)
- Sorting/filtering rules
- Desired URL and route pattern

## Procedure
1. **Define the action**
   - Add an Index/List action in the controller.
   - Use EF Core LINQ queries and Include() when needed.
2. **Shape the data**
   - Choose entity vs view model depending on display needs.
   - Apply ordering and filters.
3. **Create the view**
   - Add Views/<Entity>/Index.cshtml.
   - Render a table, list, or card grid.
4. **Link navigation**
   - Add links to details/edit/create routes.
5. **Verify**
   - Confirm routes and view names match conventions.

## Output Expectations
- List page loads without null references.
- Displays requested fields and related data.
- Consistent ordering and optional filters.

## Tips
- Keep list pages fast by selecting only needed columns.
- Use view models for computed or combined fields.
- Add empty-state messaging for zero results.
