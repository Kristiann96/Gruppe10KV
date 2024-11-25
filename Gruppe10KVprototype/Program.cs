using AuthDataAccess.Context;
using DataAccess;
using Interface;
using Logic;
using Interfaces;
using LogicInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using AuthInterface;
using AuthDataAccess.Extensions;
using AuthDataAccess.Services;
using Services;
using ServicesInterfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Gruppe10KVprototype.Controllers.HomeControllers;
using Gruppe10KVprototype.Controllers.InnmelderControllers;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddAntiforgery(options => {
    options.HeaderName = "X-CSRF-TOKEN";
});


// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Add("/Views/Innmelder/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Saksbehandler/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Home/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Map/{1}/{0}.cshtml");
    });

// Register DapperDBConnectionDummy as a service
builder.Services.AddScoped<DapperDBConnection>();

//Registrering av repos og interfaces, og logic og logicinterfaces
builder.Services.AddScoped<IGeometriRepository, GeometriRepository>();
builder.Services.AddHttpClient<IKommuneAPILogic, KommuneAPILogic>();
builder.Services.AddScoped<IInnmeldingRepository, InnmeldingRepository>();
builder.Services.AddScoped<IVurderingRepository, VurderingRepository>();
builder.Services.AddScoped<IEnumLogic, EnumLogic>();
builder.Services.AddScoped<IDataSammenstillingSaksBRepository, DataSammenstillingSaksBRepository>();
/*builder.Services.AddScoped<IGjesteinnmelderRepository, GjesteinnmelderRepository>();*/
builder.Services.AddScoped<ITransaksjonsRepository, TransaksjonsRepository>();
builder.Services.AddScoped<IInnmelderRepository, InnmelderRepository>();
builder.Services.AddScoped<ISaksbehandlerRepository, SaksbehandlerRepository>();

//Registrering av services og interfaces
builder.Services.AddScoped<IOppdatereInnmeldingService, OppdatereInnmeldingService>();



//og logic og logicinterfaces
builder.Services.AddScoped<IInnmeldingLogic, InnmeldingLogic>();

//AuthService registrering
builder.Services.AddScoped<IAuthService, AuthService>();

//HttpContextAccessor for AuthService
builder.Services.AddHttpContextAccessor();

// LoggInn
// DbContext for Identity
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MariaDbConnection_login_server"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MariaDbConnection_login_server"))
    ));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        // Passordkrav
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        // Epost-innstillinger
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

        // SignIn-innstillinger
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// Konfigurering av authentication defaults
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/LoggInn/VisLoggInnSide";
    options.AccessDeniedPath = "/Authorization/AccessDenied";
});

var app = builder.Build();

await IdentityDataInitializer.InitializeRoles(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Riktig rekkefølge for auth middleware
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseStatusCodePages();

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();