using AuthDataAccess.Context;
using DataAccess;
using Interface;
using Logic;
using Interfaces;
using LogicInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using AuthenticationService = AuthDataAccess.Services.AuthenticationService;
using AuthInterface;
using AuthDataAccess.Extensions;


var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<DapperDBConnectionDummy>(); //slettes f�r launch
builder.Services.AddScoped<DapperDBConnection>();

//Registrering av repos og interfaces, og logic og logicinterfaces
builder.Services.AddScoped<IIncidentFormRepository, IncidentFormRepository>(); //slettes før launch
builder.Services.AddScoped<IInnmeldingERepository, InnmeldingERepository>(); //slettes før launch
builder.Services.AddScoped<IGeometriRepository, GeometriRepository>();
builder.Services.AddHttpClient<IKartverketAPILogic, KartverketAPILogic>();
builder.Services.AddScoped<IInnmeldingRepository, InnmeldingRepository>();
builder.Services.AddScoped<IVurderingRepository, VurderingRepository>();
builder.Services.AddScoped<IEnumLogic, EnumLogic>();
builder.Services.AddScoped<IDataSammenstillingSaksBRepository, DataSammenstillingSaksBRepository>();
builder.Services.AddScoped<IGjesteinnmelderRepository, GjesteinnmelderRepository>();
builder.Services.AddScoped<ITransaksjonsRepository, TransaksjonsRepository>();

//og logic og logicinterfaces
builder.Services.AddScoped<IInnmeldingOpprettelseLogic, InnmeldingOpprettelseLogic>();

//AuthenticationService registrering
builder.Services.AddScoped<IAuthService, AuthenticationService>();

//HttpContextAccessor for AuthenticationService
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

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "innmelderSkjema",
    pattern: "form/{action=Form}/{id?}",
    defaults: new { controller = "InnmelderSkjemaIncidentForm" });

app.Run();