import { Component, inject, signal } from "@angular/core";
import { Router } from "@angular/router";
import { FormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatSelectModule } from "@angular/material/select";
import { AuthApi } from "../../core/auth-api";
import { I18n } from "../../core/i18n";
import { Theme } from "../../core/theme";
import { SessionStore } from "../../core/session-store";
import { SessionInfo } from "../../core/models";

/**
 * Conversion de PAGE_Splash. Les deux etapes reproduisent
 * CELL_Connect..Plan = 1 et 2 du WebDev.
 */
@Component({
    selector: "app-login",
    imports: [
        FormsModule,
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule,
        MatProgressSpinnerModule,
        MatSelectModule
    ],
    templateUrl: "./login.html",
    styleUrl: "./login.scss"
})
export class LoginPage {
    private readonly api = inject(AuthApi);
    private readonly router = inject(Router);
    private readonly store = inject(SessionStore);
    readonly i18n = inject(I18n);
    readonly theme = inject(Theme);

    readonly etape = signal<1 | 2>(1);
    readonly identifiant = signal("");
    readonly motDePasse = signal("");
    readonly erreur = signal("");
    readonly chargement = signal(false);

    readonly perimetres = signal<string[]>([]);
    readonly perimetre = signal("");
    readonly departements = signal<string[]>([]);
    readonly departement = signal("");

    readonly session = signal<SessionInfo | null>(null);

    async seConnecter(): Promise<void> {
        this.erreur.set("");
        this.chargement.set(true);
        try {
            const r = await this.api.login(this.identifiant(), this.motDePasse());
            if (r.outcome === "denied") {
                this.erreur.set(r.message ?? "");
                return;
            }
            if (r.outcome === "needsScopeSelection") {
                this.perimetres.set(r.scopes ?? []);
                this.etape.set(2);
                return;
            }
            await this.etablir(r.session ?? null);
        } finally {
            this.chargement.set(false);
        }
    }

    async changerPerimetre(nom: string): Promise<void> {
        this.perimetre.set(nom);
        this.departement.set("");
        this.departements.set(await this.api.departments(nom));
    }

    async valider(): Promise<void> {
        this.chargement.set(true);
        try {
            const r = await this.api.confirmScope(this.perimetre(), this.departement());
            await this.etablir(r.session ?? null);
        } finally {
            this.chargement.set(false);
        }
    }

    retour(): void {
        this.etape.set(1);
        this.erreur.set("");
        this.perimetre.set("");
        this.departement.set("");
    }

    /**
     * Enregistre la session puis affiche le menu, comme
     * PAGE_Prince.Affiche() en fin de PAGE_Splash.
     */
    private async etablir(s: SessionInfo | null): Promise<void> {
        this.session.set(s);
        if (s) {
            this.store.ouvrir(s);
            await this.router.navigate(["/accueil"]);
        }
    }
}
