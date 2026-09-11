import { Injectable, effect, signal } from "@angular/core";

export type ModeTheme = "clair" | "sombre";

const CLE = "aegis.theme";

/**
 * Gestion du mode clair / sombre.
 *
 * Le mode est applique par une classe sur l element racine, memorise dans le
 * navigateur, et initialise sur la preference du systeme d exploitation au
 * premier lancement.
 */
@Injectable({ providedIn: "root" })
export class Theme {
    readonly mode = signal<ModeTheme>(Theme.initial());

    constructor() {
        effect(() => {
            const m = this.mode();
            const racine = document.documentElement;
            racine.classList.toggle("sombre", m === "sombre");
            racine.style.colorScheme = m === "sombre" ? "dark" : "light";
            localStorage.setItem(CLE, m);
        });
    }

    basculer(): void {
        this.mode.update(m => (m === "clair" ? "sombre" : "clair"));
    }

    private static initial(): ModeTheme {
        const enregistre = localStorage.getItem(CLE);
        if (enregistre === "clair" || enregistre === "sombre") {
            return enregistre;
        }
        const systeme = window.matchMedia("(prefers-color-scheme: dark)").matches;
        return systeme ? "sombre" : "clair";
    }
}