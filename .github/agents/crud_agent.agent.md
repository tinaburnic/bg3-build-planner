---
name: crud_agent
description: "Use when: building ASP.NET MVC CRUD for a new entity with Create/Edit models, validation, soft delete, AJAX search, and consistent UI patterns based on Character/Build implementations. Includes controller actions, views, and reusable components."
---

# CRUD Agent Blueprint (Character/Build Standard)

Use this agent to scaffold full CRUD for a new entity following the project's established patterns.

## Inputs Needed
- Entity name (singular + plural)
- Primary fields to edit
- Required/optional fields and validation rules
- Related entities (foreign keys) and selection UI (autocomplete/select)
- Desired routes (if not default)
- Soft delete requirement (default: yes)

## Steps (Standard)
1. **Data model**
   - Add `DeletedAt` nullable timestamp to entity if soft delete required.
   - Add migration and update database.

2. **Query helpers**
   - Add `.Active()` extension to filter soft-deleted records.
   - Update list/detail queries to use `.Active()`.

3. **View models**
   - Create `Models/<Entity>/<Entity>CreateModel.cs` and `EditModel.cs`.
   - Apply data annotations for validation.

4. **Controller**
   - Index: load `.Active()` records.
   - Details: load `.Active()` record.
   - Create GET/POST: map to entity; set `CreatedAt = DateTime.UtcNow`.
   - Edit GET/POST: map entity to edit model; use `TryUpdateModelAsync`.
   - Delete POST: set `DeletedAt = DateTime.UtcNow`.
   - Search + Autocomplete endpoints: return JSON (id/text for autocomplete).

5. **Views**
   - Create/Edit views with validation summary and `_ValidationScriptsPartial`.
   - Index view with search input, live search hooks, and reveal animation.
   - Details view with Edit/Delete actions and fade-remove target.
   - Use shared partials: `_AutocompleteInput`, `_DateTimePicker` where needed.

6. **Client-side**
   - Use live-search `data-live-search-*` attributes.
   - Use `data-reveal-container` / `data-reveal-item` for staggered reveal.
   - Use `data-delete-form` + `data-delete-target` for delete fade.

## Templates (Snippets)

### Create Model
```csharp
using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Models.<Entity>
{
    public class <Entity>CreateModel
    {
        [Required]
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;
    }
}
```

### Edit Model
```csharp
using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Models.<Entity>
{
    public class <Entity>EditModel
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;
    }
}
```

### Controller Actions (Patterns)
```csharp
[HttpGet]
public IActionResult Create() => View(new <Entity>CreateModel());

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(<Entity>CreateModel model) {
    if (!ModelState.IsValid) return View(model);
    var entity = new <Entity> { /* map fields */, CreatedAt = DateTime.UtcNow };
    _dbContext.<Entities>.Add(entity);
    _dbContext.SaveChanges();
    return RedirectToAction(nameof(Details), new { id = entity.Id });
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id) {
    var entity = await _dbContext.<Entities>.Active().FirstOrDefaultAsync(e => e.Id == id);
    if (entity == null) return NotFound();
    if (await TryUpdateModelAsync(entity, "", e => e.Title)) {
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }
    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id) {
    var entity = await _dbContext.<Entities>.Active().FirstOrDefaultAsync(e => e.Id == id);
    if (entity == null) return NotFound();
    entity.DeletedAt = DateTime.UtcNow;
    await _dbContext.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}
```

### Search + Autocomplete
```csharp
[HttpGet]
public IActionResult Search(string? term) {
    var results = _dbContext.<Entities>.Active()
        .Where(e => EF.Functions.Like(e.Name, $"%{term}%"))
        .Select(e => new { e.Id, e.Name })
        .ToList();
    return Json(results);
}

[HttpGet]
public IActionResult Autocomplete(string? term) {
    var results = _dbContext.<Entities>.Active()
        .Where(e => EF.Functions.Like(e.Name, $"%{term}%"))
        .Select(e => new { Id = e.Id, Text = e.Name })
        .Take(10)
        .ToList();
    return Json(results);
}
```

### Index View Hooks
```html
<section data-live-search
         data-live-search-input="#entity-search"
         data-live-search-results="[data-entity-grid]"
         data-live-search-empty="[data-entity-empty]"
         data-live-search-url="@Url.Action("Search", "<Entity>")"
         data-live-search-renderer="render<Entity>Card">

    <div data-entity-grid data-reveal-container>
        <article data-reveal-item>...</article>
    </div>
</section>
```

## Notes
- Keep styles consistent with Character/Build (use shared classes and existing CSS).
- Use `_AutocompleteInput` for FK selection to avoid manual ID entry.
- Always keep soft delete filters in Index/Details/Edit queries.
