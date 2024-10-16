using DataAccess;
using Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register DapperDBConnection as a service
builder.Services.AddScoped<DapperDBConnection>();

//Registrering av repos og interfaces
builder.Services.AddScoped<IInnmelderRepository, InnmelderRepository>();
builder.Services.AddScoped<ISaksbehandlerRepository, SaksbehandlerRepository>();


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

// Definer routing for HomeController og InnmelderSkjemaController
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

// Route for InnmelderSkjemaController
app.MapControllerRoute(
    "innmelderSkjema",
    "form/{action=Form}/{id?}",
    new { controller = "InnmelderSkjema" });


app.Run();