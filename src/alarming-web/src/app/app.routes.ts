import { Routes } from "@angular/router";

export const routes: Routes = [
    { path: "", redirectTo: "login", pathMatch: "full" },
    {
        path: "login",
        loadComponent: () => import("./features/login/login").then(m => m.LoginPage)
    },
    {
        path: "accueil",
        loadComponent: () => import("./features/accueil/accueil").then(m => m.AccueilPage)
    },
    { path: "**", redirectTo: "login" }
];
