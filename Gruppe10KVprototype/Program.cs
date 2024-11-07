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

// Register DapperDBConnectionDummy as a service
builder.Services.AddScoped<DapperDBConnectionDummy>(); //slettes f�r launch
builder.Services.AddScoped<DapperDBConnection>();

//Registrering av repos og interfaces, og logic og logicinterfaces
builder.Services.AddScoped<IIncidentFormRepository, IncidentFormRepository>(); //slettes før launch
builder.Services.AddScoped<IInnmeldingERepository, InnmeldingERepository>(); //slettes før launch
builder.Services.AddScoped<IGeometriRepository, GeometriRepository>();
builder.Services.AddHttpClient<IKommuneAPILogic, KommuneAPILogic>();
builder.Services.AddScoped<IInnmeldingRepository, InnmeldingRepository>();
builder.Services.AddScoped<IVurderingRepository, VurderingRepository>();
builder.Services.AddScoped<IEnumLogic, EnumLogic>();


builder.Services.AddScoped<IDataSammenstillingSaksBRepository, DataSammenstillingSaksBRepository>();
builder.Services.AddScoped<IGjesteinnmelderRepository, GjesteinnmelderRepository>();
builder.Services.AddScoped<ITransaksjonsRepository, TransaksjonsRepository>();

//og logic og logicinterfaces
builder.Services.AddScoped<IInnmeldingOpprettelseLogic, InnmeldingOpprettelseLogic>();

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
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseAuthentication();

// Definer routing for HomeController og InnmelderSkjemaIncidentFormController
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

// Route for InnmelderSkjemaIncidentFormController
app.MapControllerRoute(
    "innmelderSkjema",
    "form/{action=Form}/{id?}",
    new { controller = "InnmelderSkjemaIncidentForm" });


app.Run();