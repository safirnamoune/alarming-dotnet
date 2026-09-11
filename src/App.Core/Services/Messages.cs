namespace App.Core.Services;

/// <summary>
/// Messages utilisateur repris MOT POUR MOT du code WebDev, fautes
/// d'orthographe incluses. Regle de la phase 1 : migrer sans modifier.
/// La correction est prevue en phase 2.
/// </summary>
public static class Messages
{
    // PAGE_Splash, BTN_Submit_1st : HEnDehors(REQ_SysUsers)
    public const string NotAuthorized =
        "You are not authorized to access this application. " +
        "Please contact the application administrator";

    // PAGE_Splash, BTN_Submit_1st et BTN_Submit_2nd
    // "valids" au lieu de "valid" : faute presente dans l'original.
    public const string InvalidCredentials =
        "Identification informations not valids";

    // ProceduresServeur.gsMessageErreurExecReq
    public const string QueryError =
        "Erreur lors de l'execution de la requete";
}
