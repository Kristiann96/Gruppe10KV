using Gruppe10KVprototype.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DBContext as a service
builder.Services.AddScoped<IncidentFormDBContext>();
builder.Services.AddScoped<AdviserFormDBContext>();

// Adding ApplicationDbContext as a service to allow our application to use CaseService for database operations and retrieving user cases.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MariaDbConnection"),
    new MySqlServerVersion(new Version(11, 5, 2))));

// Register CaseService
builder.Services.AddScoped<CaseService>();


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

// Define routing for HomeController and IncidentFormController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "incidentForm",
    pattern: "form/{action=Form}/{id?}",
    defaults: new { controller = "IncidentForm" });

app.Run();