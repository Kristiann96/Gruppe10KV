using Microsoft.EntityFrameworkCore;
using DataAccess;
using Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register DBContext as a service
builder.Services.AddScoped<DapperDBConnection>();
builder.Services.AddScoped<IInnmelderRepository, InnmelderRepository>();

builder.Services.AddScoped<DapperDBConnection>();
builder.Services.AddScoped<ISaksbehandlerRepository, SaksbehandlerRepository>();

// Adding ApplicationDbContext as a service to allow our application to use CaseService for database operations and retrieving user cases.
/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MariaDbConnection"),
    new MySqlServerVersion(new Version(11, 5, 2))));

// Register CaseService
builder.Services.AddScoped<CaseService>();*/


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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Route for InnmelderSkjemaController
app.MapControllerRoute(
    name: "innmelderSkjema",
    pattern: "form/{action=Form}/{id?}",
    defaults: new { controller = "InnmelderSkjema" });


app.Run();