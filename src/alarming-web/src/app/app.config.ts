import {
    ApplicationConfig,
    provideBrowserGlobalErrorListeners,
    provideZonelessChangeDetection
} from "@angular/core";
import { provideRouter } from "@angular/router";
import { provideHttpClient, withFetch } from "@angular/common/http";
import { routes } from "./app.routes";
import { AuthApi, MockAuthApi } from "./core/auth-api";
import { MockPrivilegesApi, PrivilegesApi } from "./core/privileges-api";

export const appConfig: ApplicationConfig = {
    providers: [
        provideBrowserGlobalErrorListeners(),
        provideZonelessChangeDetection(),
        provideRouter(routes),
        provideHttpClient(withFetch()),

        // Travail hors reseau. Au bureau, remplacer MockAuthApi par HttpAuthApi
        // et MockPrivilegesApi par HttpPrivilegesApi : rien d autre ne change.
        { provide: AuthApi, useClass: MockAuthApi },
        { provide: PrivilegesApi, useClass: MockPrivilegesApi }
    ]
};
