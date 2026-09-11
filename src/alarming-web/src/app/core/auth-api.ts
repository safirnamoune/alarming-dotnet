import { Injectable } from "@angular/core";
import { LoginResponse, SessionInfo } from "./models";

/**
 * Contrat d authentification. Les composants ne dependent que de cette
 * classe abstraite, jamais d une implementation concrete.
 */
@Injectable()
export abstract class AuthApi {
    abstract login(login: string, password: string): Promise<LoginResponse>;
    abstract departments(scopeName: string): Promise<string[]>;
    abstract confirmScope(scopeName: string, deptName: string): Promise<LoginResponse>;
}

const attendre = (ms: number) => new Promise(r => setTimeout(r, ms));

/**
 * Implementation simulee, utilisee hors du reseau Djezzy.
 * Jeux d essai :
 *   - "mono"  : un seul perimetre, connexion directe
 *   - "multi" : plusieurs perimetres, selection requise
 *   - autre   : refus, avec le message iso-fonctionnel du WebDev
 */
@Injectable()
export class MockAuthApi extends AuthApi {
    private readonly perimetres = ["ALGER", "ORAN", "CONSTANTINE"];

    private readonly departements: Record<string, string[]> = {
        ALGER: ["CORE NETWORK", "RADIO ACCESS", "TRANSMISSION"],
        ORAN: ["RADIO ACCESS", "TRANSMISSION"],
        CONSTANTINE: ["CORE NETWORK", "SUPERVISION"]
    };

    override async login(login: string, _password: string): Promise<LoginResponse> {
        await attendre(600);
        const code = login.trim().toLowerCase();

        if (code === "multi") {
            return { outcome: "needsScopeSelection", scopes: this.perimetres };
        }

        if (code === "mono") {
            return { outcome: "success", session: this.session("ALGER", "CORE NETWORK") };
        }

        return {
            outcome: "denied",
            message:
                "You are not authorized to access this application. " +
                "Please contact the application administrator"
        };
    }

    override async departments(scopeName: string): Promise<string[]> {
        await attendre(300);
        return this.departements[scopeName] ?? [];
    }

    override async confirmScope(scopeName: string, deptName: string): Promise<LoginResponse> {
        await attendre(400);
        return { outcome: "success", session: this.session(scopeName, deptName) };
    }

    private session(scopeName: string, deptName: string): SessionInfo {
        return {
            userCode: "2939",
            userName: "NAMOUNE Safir",
            userLogin: "SAFIR.NAMOUNE",
            userMail: "safir.namoune@otalgerie.com",
            userAgent: "SYSTEM",
            isAdmin: true,
            scopeId: 1,
            scopeName,
            teamId: 12,
            teamName: deptName,
            sessionId: 691,
            language: "fr"
        };
    }
}