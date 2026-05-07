---
name: routing-semantic-urls
description: 'Use when: designing ASP.NET MVC routing, adding attribute routing, creating semantic URLs, and documenting route-to-controller mappings.'
argument-hint: 'Describe the routes you want, controller/actions involved, and any constraints or parameters.'
user-invocable: true
disable-model-invocation: false
---

# Routing and Semantic URLs

## When to Use
- Add or refactor attribute routing in MVC controllers
- Create human-readable, semantic URL patterns
- Add route constraints (int, slug, optional parameters)
- Map routes to views and document them
- Maintain canonical and alias routes

## Inputs to Ask For
- Desired URL patterns and parameters
- Controller/action names and expected views
- Required constraints (int, guid, slug, etc.)
- Whether to keep conventional routes alongside attribute routes

## Procedure
1. **Inventory current routes**
   - Review controller actions and existing route attributes.
   - Note conventional routing setup in Program.cs.
2. **Define semantic URLs**
   - Choose readable path segments.
   - Add route constraints for IDs and optional parameters.
3. **Apply attribute routing**
   - Add `[Route("...")]` at controller level.
   - Add `[HttpGet("...")]` or other verbs at action level.
4. **Resolve conflicts**
   - Ensure attribute routes do not collide with conventional routes.
   - Add `MapControllers()` if attribute routing is used.
5. **Document the sitemap**
   - List each route with URL, controller, action, and view.

## Output Expectations
- Clear, semantic URL patterns in controllers.
- No route conflicts or ambiguous matches.
- Sitemap documentation updated.

## Tips
- Keep canonical routes short and stable.
- Use query strings for filters and sorting.
- Prefer attribute routing for public-facing URLs.
