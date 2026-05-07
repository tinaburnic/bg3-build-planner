---
name: edit-form-pages
description: 'Use when: creating or updating create/edit form pages in ASP.NET MVC, including controller actions, validation, and Razor form layout.'
argument-hint: 'Provide the entity name, fields to edit, validation rules, and route expectations.'
user-invocable: true
disable-model-invocation: false
---

# Edit and Create Form Pages

## When to Use
- Add Create and Edit actions for an entity
- Build form views with validation
- Wire up POST actions and model binding
- Add dropdowns for related entities

## Inputs to Ask For
- Entity name and editable fields
- Required validation rules
- Related entity selections (dropdowns)
- Desired routes and view names

## Procedure
1. **Controller actions**
   - Add GET actions for Create and Edit.
   - Add POST actions with model binding and validation.
2. **Validation**
   - Use data annotations or Fluent Validation as required.
   - Return the view with validation errors when invalid.
3. **Form views**
   - Add Views/<Entity>/Create.cshtml and Views/<Entity>/Edit.cshtml.
   - Use tag helpers (`asp-for`, `asp-validation-for`).
4. **Related data**
   - Populate select lists and bind foreign keys.
5. **Persistence**
   - Save changes via DbContext and redirect on success.

## Output Expectations
- Forms display field labels, validation, and errors.
- POST actions handle invalid and valid states.
- Data saved and user redirected appropriately.

## Tips
- Use view models for complex forms.
- Keep Create/Edit forms consistent in layout.
- Validate server-side even if client-side exists.
