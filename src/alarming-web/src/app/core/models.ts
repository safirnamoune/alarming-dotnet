// Reprend exactement les champs de App.Core.Session.UserSession,
// a l exception des cles RSA qui ne quittent jamais le serveur.
export interface SessionInfo {
    userCode: string;
    userName: string;
    userLogin: string;
    userMail: string;
    userAgent: string;
    isAdmin: boolean;
    scopeId: number;
    scopeName: string;
    teamId: number;
    teamName: string;
    sessionId: number;
    language: string;
}

// Les trois issues de App.Core.Services.LoginResult.
export type LoginOutcome = "success" | "needsScopeSelection" | "denied";

export interface LoginResponse {
    outcome: LoginOutcome;
    message?: string;
    scopes?: string[];
    session?: SessionInfo;
}