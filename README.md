# Alarming — migration WebDev vers Blazor

Squelette de la solution .NET pour la migration du projet WebDev `Alarming_UX`.
Phase 1 : **migrer sans modifier**.

## Contenu

| Projet | Rôle | Remplace dans WebDev |
| --- | --- | --- |
| `App.Web` | Blazor Server, composants `.razor` | Les pages et leurs champs |
| `App.Core` | Services métier, modèles, état de session | Les traitements et `ProcéduresServeur` |
| `App.Data` | Requêtes Dapper sur PostgreSQL | L'analyse et les requêtes `REQ_*` |

## Démarrer

```bash
# 1. Restaurer les paquets NuGet
dotnet restore

# 2. Renseigner la chaîne de connexion (ne pas committer le mot de passe)
cd src/App.Web
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Alarming" \
  "Host=172.16.59.238;Database=alarmingdb;Username=xxx;Password=xxx;SearchPath=alarming_schema"

# 3. Lancer
dotnet run --project src/App.Web
```

Dans VS Code : installez **C# Dev Kit**, ouvrez le dossier, puis F5.

## Ce qui est déjà converti

- `PAGE_Splash` → `Components/Pages/Login.razor` (les deux étapes, scope et groupe)
- `REQ_SysUsers` → `UserQueries.GetByLoginAsync`
- `REQ_ajout_audit_access` → `AuditQueries.LogAccessAsync`
- `REQ_Rch_User_Keys` / `REQ_Ajout_User_Keys` → `UserKeyQueries`
- `REQ_Rch_Users_Scopes_View` → `UserQueries.GetScopeDepartmentsAsync`
- `Get_User_keys()` et `init_scopes_departements()` → `AuthService`
- Globales de `ProcéduresServeur` → `UserSession` (Scoped)
- `NavigateurType()` → `ClientInfoService`

## Prochaines étapes

1. `PAGE_Prince` (écran d'accueil réel, 28 champs)
2. `PAGE_Main` (31 champs), `PAGE_Outage` (38 champs)
3. `ProcéduresServeur` découpée en services (7 380 lignes)
4. `PAGE_Ticketing` (203 champs) avec l'intégration CM

## Avertissements

- **Aucun accès à votre base n'a été possible** : les noms de tables et de vues
  utilisés dans `App.Data` sont déduits du dictionnaire de données et du code
  WebDev. Vérifiez notamment `users_scopes_view`, `scopes` et `user_keys`.
- **Ce code n'a pas été compilé** (pas de SDK .NET disponible lors de la
  génération). Attendez-vous à quelques ajustements au premier `dotnet build`.
- Voir `docs/registre-anomalies.md` : le mot de passe n'est **pas** vérifié,
  conformément à la règle « migrer sans modifier ».
