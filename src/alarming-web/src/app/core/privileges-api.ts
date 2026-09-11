import { Injectable } from "@angular/core";

/** Resultat de REQ_Rch_Access_Privileges. */
export type Privileges = {
    read: boolean;
    write: boolean;
};

/**
 * Controle d acces par ecran.
 *
 * Conversion de REQ_Rch_Access_Privileges, appelee dans PAGE_Prince avant
 * chaque navigation avec les parametres paramUserId, Paramdeptid,
 * Paramscope_id et paramPage_Id.
 *
 * L implementation reelle interrogera App.Api. Comme pour AuthApi, seule la
 * ligne de fourniture dans app.config.ts changera.
 */
export abstract class PrivilegesApi {
    abstract check(moduleId: string): Promise<Privileges>;
}

@Injectable()
export class MockPrivilegesApi extends PrivilegesApi {
    /**
     * Jeu d essai hors reseau. "consultation" est volontairement en lecture
     * seule pour eprouver le chemin gbLectureSeule, et "admin" est refuse pour
     * eprouver le message de privileges insuffisants.
     */
    private readonly table: Record<string, Privileges> = {
        ticketing: { read: true, write: true },
        outage: { read: true, write: true },
        dashboard: { read: true, write: true },
        sw: { read: true, write: true },
        consultation: { read: true, write: false },
        admin: { read: false, write: false }
    };

    async check(moduleId: string): Promise<Privileges> {
        await new Promise(r => setTimeout(r, 220));
        return this.table[moduleId] ?? { read: false, write: false };
    }
}
