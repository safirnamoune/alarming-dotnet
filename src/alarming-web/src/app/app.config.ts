import {
    ApplicationConfig,
    provideBrowserGlobalErrorListeners,
    provideZonelessChangeDetection
} from "@angular/core";
import { provideRouter } from "@angular/router";
import { provideHttpClient, withFetch } from "@angular/common/http";
import { routes } from "./app.routes";
import { AuthApi, MockAuthApi } from "./core/auth-api";
import { AdminOnlyPrivilegesApi, PrivilegesApi } from "./core/privileges-api";

export const appConfig: ApplicationConfig = {
    providers: [
        provideBrowserGlobalErrorListeners(),
        provideZonelessChangeDetection(),
        provideRouter(routes),
        provideHttpClient(withFetch()),

        // Travail hors reseau : remplacer MockAuthApi par HttpAuthApi une fois
        // App.Api en service. Rien d autre ne change.
        { provide: AuthApi, useClass: MockAuthApi },

        // Privileges : isadmin seul, decision du 12/09/2026 (anomalie A7).
        // Cette ligne n aura pas a changer, le controle etant porte par la
        // session et non par un appel reseau.
        { provide: PrivilegesApi, useClass: AdminOnlyPrivilegesApi }
    ]
};
