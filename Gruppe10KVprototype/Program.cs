using Models.DbContexts;
using AdviserFormDBContext = Models.DbContexts.AdviserFormDBContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrer DBContext som en service
builder.Services.AddScoped<IncidentFormDBContext>();
builder.Services.AddScoped<AdviserFormDBContext>();
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

// Definer routing for HomeController og IncidentFormController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "incidentForm",
    pattern: "form/{action=Form}/{id?}",
    defaults: new { controller = "IncidentForm" });

app.Run();
