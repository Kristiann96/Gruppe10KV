using DataAccess;
using Interface;
using Logic;
using Interfaces;
using LogicInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;

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

// Legg til DbContext for Identity
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MariaDbConnection_login_server"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MariaDbConnection_login_server"))
    ));

// Legg til Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// Dine eksisterende service registreringer
builder.Services.AddScoped<DapperDBConnection>();
builder.Services.AddScoped<DapperDBConnectionDummy>();
builder.Services.AddScoped<IIncidentFormRepository, IncidentFormRepository>();
builder.Services.AddScoped<IInnmeldingERepository, InnmeldingERepository>();
builder.Services.AddScoped<IGeometriRepository, GeometriRepository>();
builder.Services.AddHttpClient<IKartverketAPILogic, KartverketAPILogic>();
builder.Services.AddScoped<IInnmeldingRepository, InnmeldingRepository>();
builder.Services.AddScoped<IInnmeldingEnumLogic, InnmeldingEnumLogic>();
builder.Services.AddScoped<IDataSammenstillingSaksBRepository, DataSammenstillingSaksBRepository>();
builder.Services.AddScoped<IGjesteinnmelderRepository, GjesteinnmelderRepository>();
builder.Services.AddScoped<ITransaksjonsRepository, TransaksjonsRepository>();
builder.Services.AddScoped<IInnmeldingOpprettelseLogic, InnmeldingOpprettelseLogic>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Legg til Authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    "innmelderSkjema",
    "form/{action=Form}/{id?}",
    new { controller = "InnmelderSkjemaIncidentForm" });

app.Run();