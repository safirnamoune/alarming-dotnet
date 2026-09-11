# Registre des anomalies

Règle de la phase 1 : **migrer sans modifier**. Les anomalies ci-dessous sont
volontairement reproduites. Elles sont consignées ici pour qu'aucune ne soit
oubliée, ni corrigée par accident.

| N° | Anomalie | Origine | Phase 1 | Phase 2 |
| --- | --- | --- | --- | --- |
| A1 | Mot de passe jamais vérifié : LDAP neutralisé par `SI 1=1`, `pUserPassword` forcé à `Null` | `PAGE_Splash`, `BTN_Submit_1st` | Reproduit à l'identique (`AuthService.LoginAsync`) | Entra ID ou ASP.NET Core Identity |
| A2 | `gsAdminPassword = "231276"` en clair | `ProcéduresServeur` | Déplacé dans `appsettings.json`, valeur inchangée | Suppression |
| A3 | `gsDefaultPassword = "go"` | `ProcéduresServeur` | Idem | Suppression |
| A4 | Clé privée RSA stockée en base et chargée en session | `Get_User_keys()` | Reproduit | Coffre de secrets, ou suppression du besoin |
| A5 | `SELECT max(Id_Session)` après insertion | `BTN_Submit_1st` | **Exception autorisée** : `RETURNING id_session` | — |
| A6 | Adresses e-mail de service en dur | `ProcéduresServeur` | Déplacées en configuration, valeurs inchangées | — |
| A7 | Nom de cryptage en dur `"CrypPWSuPlusW"` | `ProcéduresServeur` | Reproduit si utilisé | À revoir |
| A8 | 161 erreurs et 850 informations à la compilation WebDev | Projet | Non traité | Nettoyage du code mort |
| A9 | Fautes dans les messages utilisateur (« not valids ») | `PAGE_Splash` | Reproduites à la lettre | Correction |

## Les trois seules exceptions autorisées

1. **Mécanismes de plateforme** — cookies, AJAX, plans de cellule, session :
   remplacés par leurs équivalents natifs Blazor.
2. **Secrets et paramètres en dur** — déplacés en configuration, valeurs
   rigoureusement inchangées.
3. **Anti-patterns à risque nul** — `RETURNING id` au lieu de `SELECT max(id)`.
   Résultat identique, aucun impact visible.

Toute autre divergence est un défaut de migration.
