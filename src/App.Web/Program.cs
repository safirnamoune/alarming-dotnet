using App.Core.Abstractions;
using App.Core.Services;
using App.Core.Session;
using App.Data.Queries;
using App.Web.Components;
using App.Web.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Blazor Server ---
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

// --- Bibliotheque UI ---
builder.Services.AddMudServices();

// --- Acces a la requete HTTP (IP et User-Agent pour audit_access) ---
builder.Services.AddHttpContextAccessor();

// --- PostgreSQL : la base alarmingdb existante n'est PAS modifiee ---
builder.Services.AddNpgsqlDataSource(
    builder.Configuration.GetConnectionString("Alarming")
    ?? throw new InvalidOperationException(
        "Chaine de connexion 'Alarming' absente de appsettings.json"));

// --- Acces aux donnees (lecture via les vues, comme le WebDev) ---
builder.Services.AddScoped<IUserQueries, UserQueries>();
builder.Services.AddScoped<IAuditQueries, AuditQueries>();
builder.Services.AddScoped<IUserKeyQueries, UserKeyQueries>();

// --- Remplace NavigateurType() / NavigateurAdresseIP() ---
builder.Services.AddScoped<IClientInfo, ClientInfoService>();

// --- Etat de session : remplace les globales de ProceduresServeur ---
// Scoped = une instance par connexion utilisateur en Blazor Server.
builder.Services.AddScoped<UserSession>();

// --- Services metier ---
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
