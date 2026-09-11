import { Injectable, computed, signal } from "@angular/core";

const fr = {
    marque: "Aegis",
    supervision: "Supervision",
    incidents: "Incidents",
    ticketing: "Ticketing",
    connexion: "Connexion",
    identifiant: "Identifiant",
    motDePasse: "Mot de passe",
    seConnecter: "Se connecter",
    perimetre: "Perimetre",
    departement: "Departement",
    valider: "Valider",
    retour: "Retour",
    choisirPerimetre: "Selectionnez votre perimetre et votre departement",
    langue: "English",
    patienter: "Verification..."
};

const en = {
    marque: "Aegis",
    supervision: "Supervision",
    incidents: "Incidents",
    ticketing: "Ticketing",
    connexion: "Sign in",
    identifiant: "User code",
    motDePasse: "Password",
    seConnecter: "Sign in",
    perimetre: "Scope",
    departement: "Department",
    valider: "Confirm",
    retour: "Back",
    choisirPerimetre: "Select your scope and department",
    langue: "Francais",
    patienter: "Checking..."
};

export type CleTraduction = keyof typeof fr;

/**
 * Remplace ProceduresServeur.gnLangue_Utilisee.
 *
 * Les messages d erreur renvoyes par l API ne sont PAS traduits : ils sont
 * repris au mot pres du code WebDev, en anglais, conformement a la regle
 * iso-fonctionnelle de la phase 1.
 */
@Injectable({ providedIn: "root" })
export class I18n {
    readonly langue = signal<"fr" | "en">("fr");

    private readonly dictionnaire = computed(() => (this.langue() === "fr" ? fr : en));

    t(cle: CleTraduction): string {
        return this.dictionnaire()[cle];
    }

    basculer(): void {
        this.langue.update(l => (l === "fr" ? "en" : "fr"));
    }
}