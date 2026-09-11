import {
    ApplicationConfig,
    provideBrowserGlobalErrorListeners,
    provideZonelessChangeDetection
} from "@angular/core";
import { provideRouter } from "@angular/router";
import { provideHttpClient, withFetch } from "@angular/common/http";

import { routes } from "./app.routes";
import { AuthApi, MockAuthApi } from "./core/auth-api";

export const appConfig: ApplicationConfig = {
    providers: [
        provideBrowserGlobalErrorListeners(),
        provideZonelessChangeDetection(),
        provideRouter(routes),
        provideHttpClient(withFetch()),
        // Hors reseau Djezzy : implementation simulee.
        // Au bureau, remplacer MockAuthApi par HttpAuthApi.
        { provide: AuthApi, useClass: MockAuthApi }
    ]
};
