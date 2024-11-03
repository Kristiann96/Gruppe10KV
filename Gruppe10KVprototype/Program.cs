using DataAccess;
using Interface;
using Logic;
using Interfaces;
using LogicInterfaces;

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
builder.Services.AddScoped<IInnmeldingEnumLogic, InnmeldingEnumLogic>();
builder.Services.AddScoped<IDataSammenstillingSaksBRepository, DataSammenstillingSaksBRepository>();
builder.Services.AddScoped<IGjesteinnmelderRepository, GjesteinnmelderRepository>();


//og logic og logicinterfaces
builder.Services.AddScoped<IInnmeldingOpprettelseLogic, InnmeldingOpprettelseLogic>();





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