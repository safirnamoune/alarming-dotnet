import { Component, computed, inject, signal } from "@angular/core";
import { Router } from "@angular/router";
import { MatButtonModule } from "@angular/material/button";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { I18n } from "../../core/i18n";
import { Theme } from "../../core/theme";
import { SessionStore } from "../../core/session-store";
import { PrivilegesApi } from "../../core/privileges-api";

type Bilingue = { fr: string; en: string };

export type Module = {
    id: string;
    titre: Bilingue;
    sous: Bilingue;
    /** Route Angular si l ecran est deja converti. */
    route?: string;
};

/** Message verbatim de PAGE_Prince, conserve tel quel. */
const REFUS = "You do not have the necessary privileges to access this page";

@Component({
    selector: "app-accueil",
    imports: [MatButtonModule, MatProgressSpinnerModule],
    templateUrl: "./accueil.html",
    styleUrl: "./accueil.scss"
})
export class AccueilPage {
    private readonly api = inject(PrivilegesApi);
    private readonly router = inject(Router);
    readonly store = inject(SessionStore);
    readonly i18n = inject(I18n);
    readonly theme = inject(Theme);

    readonly erreur = signal("");
    readonly info = signal("");
    readonly occupe = signal("");

    readonly modules: Module[] = [
        {
            id: "ticketing",
            titre: { fr: "Tickets", en: "Ticketing" },
            sous: { fr: "Suivi et traitement des incidents", en: "Incident tracking and handling" }
        },
        {
            id: "outage",
            titre: { fr: "Coupures", en: "Outage" },
            sous: { fr: "Interruptions de service en cours", en: "Ongoing service interruptions" }
        },
        {
            id: "dashboard",
            titre: { fr: "Tableau de bord", en: "Dashboard" },
            sous: { fr: "Indicateurs et vue d ensemble", en: "Indicators and overview" }
        },
        {
            id: "sw",
            titre: { fr: "Travaux planifies", en: "Scheduled work" },
            sous: { fr: "Planning, agenda et approbations", en: "Planning, agenda and approvals" }
        },
        {
            id: "consultation",
            titre: { fr: "Consultation", en: "Consultation" },
            sous: { fr: "Etats et courriers sortants", en: "Reports and outgoing mail" }
        },
        {
            id: "admin",
            titre: { fr: "Administration", en: "Administration" },
            sous: { fr: "Utilisateurs, secteurs et departements", en: "Users, scopes and departments" }
        }
    ];

    readonly bonjour = computed(() =>
        this.i18n.langue() === "fr" ? "Bonjour" : "Welcome"
    );

    readonly session = computed(() => this.store.session());

    constructor() {
        // Equivalent du controle de contexte AWP : sans session, retour a l ecran
        // de connexion.
        if (!this.store.connecte()) {
            void this.router.navigate(["/login"]);
        }
    }

    texte(b: Bilingue): string {
        return this.i18n.langue() === "fr" ? b.fr : b.en;
    }

    /**
     * Reproduit le clic sur les vignettes de PAGE_Prince : lecture des
     * privileges, positionnement du mode lecture seule, puis affichage.
     */
    async ouvrir(m: Module): Promise<void> {
        this.erreur.set("");
        this.info.set("");
        this.occupe.set(m.id);
        try {
            const p = await this.api.check(m.id);
            const admin = this.session()?.isAdmin ?? false;

            if (!admin && !p.read && !p.write) {
                this.erreur.set(REFUS);
                return;
            }

            this.store.lectureSeule.set(!admin && p.read && !p.write);

            if (m.route) {
                await this.router.navigate([m.route]);
                return;
            }

            const seule = this.store.lectureSeule();
            this.info.set(
                this.i18n.langue() === "fr"
                    ? `Acces autorise a ${m.titre.fr}${seule ? " en lecture seule" : ""}. Ecran en cours de conversion.`
                    : `Access granted to ${m.titre.en}${seule ? " in read-only mode" : ""}. Screen conversion in progress.`
            );
        } finally {
            this.occupe.set("");
        }
    }

    /** Conversion du clic sur Link_MDL_03 : fermeture de session. */
    async deconnecter(): Promise<void> {
        this.store.fermer();
        await this.router.navigate(["/login"]);
    }
}
