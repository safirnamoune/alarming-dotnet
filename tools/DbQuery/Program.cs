using System.Text;
using Microsoft.Extensions.Configuration;
using Npgsql;

// Outil de diagnostic : execute une requete SQL et affiche le resultat en
// tableau. Sert a verifier le schema reel avant d'ecrire du code qui en depend.
//
//   dotnet run --project tools/DbQuery -- "SELECT 1"
//   dotnet run --project tools/DbQuery -- --file requete.sql

Console.OutputEncoding = Encoding.UTF8;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

var chaine = config.GetConnectionString("Alarming");
if (string.IsNullOrWhiteSpace(chaine))
{
    Console.Error.WriteLine("Chaine de connexion 'Alarming' introuvable dans les secrets.");
    return 2;
}

string? sql = null;

if (args.Length >= 2 && (args[0] == "--file" || args[0] == "-f"))
{
    if (!File.Exists(args[1]))
    {
        Console.Error.WriteLine($"Fichier introuvable : {args[1]}");
        return 2;
    }
    sql = File.ReadAllText(args[1]);
}
else if (args.Length >= 1)
{
    sql = string.Join(" ", args);
}

if (string.IsNullOrWhiteSpace(sql))
{
    Console.Error.WriteLine("Aucune requete fournie.");
    Console.Error.WriteLine("Usage : dotnet run --project tools/DbQuery -- \"SELECT 1\"");
    return 2;
}

try
{
    await using var cnx = new NpgsqlConnection(chaine);
    await cnx.OpenAsync();

    await using var cmd = new NpgsqlCommand(sql, cnx);
    cmd.CommandTimeout = 60;

    await using var lecteur = await cmd.ExecuteReaderAsync();

    var jeu = 0;
    do
    {
        if (lecteur.FieldCount == 0)
        {
            Console.WriteLine($"{lecteur.RecordsAffected} ligne(s) affectee(s).");
            continue;
        }

        if (jeu++ > 0) Console.WriteLine();

        var entetes = Enumerable.Range(0, lecteur.FieldCount)
            .Select(i => lecteur.GetName(i))
            .ToArray();

        var types = Enumerable.Range(0, lecteur.FieldCount)
            .Select(i => lecteur.GetDataTypeName(i))
            .ToArray();

        var lignes = new List<string[]>();
        while (await lecteur.ReadAsync())
        {
            var ligne = new string[lecteur.FieldCount];
            for (var i = 0; i < lecteur.FieldCount; i++)
            {
                ligne[i] = await lecteur.IsDBNullAsync(i)
                    ? "(null)"
                    : lecteur.GetValue(i)?.ToString() ?? "";
            }
            lignes.Add(ligne);
        }

        var largeurs = new int[lecteur.FieldCount];
        for (var i = 0; i < lecteur.FieldCount; i++)
        {
            largeurs[i] = Math.Max(entetes[i].Length, types[i].Length + 2);
            foreach (var l in lignes)
            {
                largeurs[i] = Math.Max(largeurs[i], Math.Min(l[i].Length, 60));
            }
        }

        string Formater(IReadOnlyList<string> cellules) =>
            "  " + string.Join("  ", cellules.Select((c, i) =>
            {
                var v = c.Length > 60 ? c[..57] + "..." : c;
                return v.PadRight(largeurs[i]);
            })).TrimEnd();

        Console.WriteLine(Formater(entetes));
        Console.WriteLine(Formater(types.Select(t => $"[{t}]").ToArray()));
        Console.WriteLine("  " + string.Join("  ", largeurs.Select(w => new string('-', w))));

        foreach (var l in lignes) Console.WriteLine(Formater(l));

        Console.WriteLine();
        Console.WriteLine($"  {lignes.Count} ligne(s).");
    }
    while (await lecteur.NextResultAsync());

    return 0;
}
catch (PostgresException ex)
{
    Console.Error.WriteLine($"Erreur PostgreSQL {ex.SqlState} : {ex.MessageText}");
    if (!string.IsNullOrWhiteSpace(ex.Detail)) Console.Error.WriteLine($"Detail : {ex.Detail}");
    if (!string.IsNullOrWhiteSpace(ex.Hint)) Console.Error.WriteLine($"Piste : {ex.Hint}");
    return 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"{ex.GetType().Name} : {ex.Message}");
    return 1;
}
