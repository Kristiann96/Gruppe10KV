using Gruppe10KVprototype.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register DBContext as a service
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

// Map Razor Pages
app.MapRazorPages();

// Define routing for HomeController, IncidentFormController, and LoginController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Route for IncidentFormController
app.MapControllerRoute(
    name: "incidentForm",
    pattern: "form/{action=Form}/{id?}",
    defaults: new { controller = "IncidentForm" });

// Ensure routing for UserPages
app.MapControllerRoute(
    name: "userPages",
    pattern: "UserPages/{controller=Login}/{action=Login}/{id?}");

app.Run();
