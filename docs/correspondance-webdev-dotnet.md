# Correspondance WebDev → .NET (projet Alarming_UX)

## Écrans

| WebDev | Champs | .NET | État |
| --- | --- | --- | --- |
| `PAGE_Splash` | 17 | `Components/Pages/Login.razor` | Converti |
| `PAGE_Prince` | 28 | `Components/Pages/Prince.razor` | Squelette |
| `PAGE_Main` | 31 | — | À faire |
| `PAGE_Outage` | 38 | — | À faire |
| `PAGE_Ticketing` | 203 | — | À faire |

## Requêtes

| WebDev | .NET | État |
| --- | --- | --- |
| `REQ_SysUsers` | `UserQueries.GetByLoginAsync` | Converti |
| `REQ_Rch_Users_Scopes_View` | `UserQueries.GetScopeDepartmentsAsync` | Converti |
| `REQ_ajout_audit_access` | `AuditQueries.LogAccessAsync` | Converti |
| `REQ_Rch_User_Keys` | `UserKeyQueries.GetAsync` | Converti |
| `REQ_Ajout_User_Keys` | `UserKeyQueries.InsertAsync` | Converti |
| 29 autres `REQ_rch_vw_*` | — | À faire |

## Fonctions WLangage

| WLangage | .NET |
| --- | --- |
| `HExécuteRequête` + `HLitPremier` / `HLitSuivant` | `conn.QueryAsync<T>()` → `List<T>` |
| `HLitRecherchePremier` | `conn.QueryFirstOrDefaultAsync<T>()` |
| `HNbEnr` | `rows.Count` |
| `HEnDehors` | fin de boucle `foreach` |
| `Majuscule()` | `ToUpperInvariant()` |
| `DateSys()` / `HeureSys()` | `DateTime.Today` / `DateTime.Now.TimeOfDay` |
| `NavigateurAdresseIP()` | `HttpContext.Connection.RemoteIpAddress` |
| `NavigateurType()` | en-tête `User-Agent` (`ClientInfoService`) |
| `CookieEcrit` | authentification par cookie ASP.NET Core |
| `CrypteGénèreCléRSA()` | `RSA.Create(2048)` |
| `PageAffiche()` | `NavigationManager.NavigateTo()` |
| `CELL_xxx..Plan` | rendu conditionnel `@if` |
| `CelluleAfficheDialogue(CelluleAttente)` | `Disabled` + `MudProgressCircular` |
| `AJAXExécute` / retours AJAX | natif en Blazor Server |
| `Erreur()` | `MudAlert` ou `ISnackbar` |
| Variables globales `g*` | `UserSession` en `Scoped` |
