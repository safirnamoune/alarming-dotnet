import { Injectable, computed, signal } from "@angular/core";
import { SessionInfo } from "./models";

const CLE = "aegis.session";

/**
 * Session applicative partagee entre les ecrans.
 *
 * Remplace les variables globales du WLangage : gsUserCode, gsTeamId,
 * gnScope_Id, gbUserAdmin et gbLectureSeule etaient portees par
 * ProceduresServeur. Elles sont ici regroupees dans un service unique.
 *
 * Le stockage de session du navigateur est utilise volontairement plutot que
 * le stockage local : la session disparait a la fermeture de l onglet, ce qui
 * reproduit le comportement du cookie gsCookieUserInfo.
 */
@Injectable({ providedIn: "root" })
export class SessionStore {
    readonly session = signal<SessionInfo | null>(SessionStore.relire());

    /** Equivalent de ProceduresServeur.gbLectureSeule. */
    readonly lectureSeule = signal(false);

    readonly connecte = computed(() => this.session() !== null);

    ouvrir(s: SessionInfo): void {
        this.session.set(s);
        sessionStorage.setItem(CLE, JSON.stringify(s));
    }

    fermer(): void {
        this.session.set(null);
        this.lectureSeule.set(false);
        sessionStorage.removeItem(CLE);
    }

    private static relire(): SessionInfo | null {
        try {
            const brut = sessionStorage.getItem(CLE);
            return brut ? (JSON.parse(brut) as SessionInfo) : null;
        } catch {
            return null;
        }
    }
}
