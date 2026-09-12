import { inject, Injectable } from "@angular/core";
import { SessionStore } from "./session-store";

/** Resultat d un controle d acces a un ecran. */
export type Privileges = {
    read: boolean;
    write: boolean;
};

/**
 * Controle d acces par ecran.
 *
 * PAGE_Prince appelait REQ_Rch_Access_Privileges avant chaque navigation, avec
 * les parametres paramUserId, Paramdeptid, Paramscope_id et paramPage_Id.
 *
 * CETTE REQUETE N EXISTE PAS dans le projet WebDev : elle est absente des 34
 * requetes exportees, et le dossier de projet porte 161 diagnostics
 * "L element 'REQ_Rch_Access_Privileges' est inconnu ou inaccessible".
 * Aucune table de privileges par page ne figure au dictionnaire de donnees.
 *
 * De surcroit, users_scopes est vide : Paramdeptid et Paramscope_id seraient
 * NULL pour tous les comptes.
 *
 * DECISION DU 12/09/2026 (anomalie A7 du registre) : s en tenir a isadmin,
 * seul controle reellement operant aujourd hui. Aucun modele de droits n est
 * invente, ce qui sortirait de la regle iso-fonctionnelle de la phase 1.
 */
export abstract class PrivilegesApi {
    abstract check(moduleId: string): Promise<Privileges>;
}

/** Ecrans reserves aux administrateurs. */
const RESERVE_ADMIN = new Set(["admin"]);

@Injectable()
export class AdminOnlyPrivilegesApi extends PrivilegesApi {
    private readonly store = inject(SessionStore);

    async check(moduleId: string): Promise<Privileges> {
        // Equivalent de gbUserAdmin.
        const admin = this.store.session()?.isAdmin ?? false;

        if (RESERVE_ADMIN.has(moduleId)) {
            return { read: admin, write: admin };
        }

        // Tout utilisateur authentifie consulte les ecrans metier ; seul un
        // administrateur y ecrit. C est le comportement observable du WebDev,
        // ou gbLectureSeule etait le seul effet du controle.
        return { read: true, write: admin };
    }
}
