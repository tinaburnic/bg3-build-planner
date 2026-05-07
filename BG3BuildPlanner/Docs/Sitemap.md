# Sitemap

This sitemap lists the HTTP routes exposed by the MVC controllers, along with the mapped controller/action and the Razor view used.

## Canonical Routes

| URL | Controller | Action | View |
| --- | --- | --- | --- |
| / | Home | Index | Views/Home/Index.cshtml |
| /Home/Privacy | Home | Privacy | Views/Home/Privacy.cshtml |
| /Home/Error | Home | Error | Views/Shared/Error.cshtml |
| /builds | Build | Index | Views/Build/Index.cshtml |
| /builds/{id} | Build | Details | Views/Build/Details.cshtml |
| /builds/character/{characterId} | Build | ByCharacter | Views/Build/Index.cshtml |
| /builds/top | Build | Top | Views/Build/Index.cshtml |
| /Character | Character | Index | Views/Character/Index.cshtml |
| /Character/Details/{id} | Character | Details | Views/Character/Details.cshtml |
| /Skill | Skill | Index | Views/Skill/Index.cshtml |
| /Skill/Details/{id} | Skill | Details | Views/Skill/Details.cshtml |

## Notes
- Build routes are explicitly defined via attribute routing and are the preferred semantic URLs.
- The default MVC route is still enabled, so conventional URLs such as /Home/Index and /Build/Index will also resolve.
- The Error action returns the shared error view (Views/Shared/Error.cshtml).
