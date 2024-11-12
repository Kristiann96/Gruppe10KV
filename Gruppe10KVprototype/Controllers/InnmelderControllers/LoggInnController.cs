using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ViewModels;



namespace Gruppe10KVprototype.Controllers.InnmelderControllers;
public class LoggInnController : Controller

{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<LoggInnController> _logger;
    public LoggInnController(SignInManager<IdentityUser> signInManager, ILoggerFactory loggerFactory)
    {
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<LoggInnController>();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoggInn(LoggInnViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            _logger.LogInformation(1, "User logged in.");
            return RedirectToAction("LandingsSide", "LandingsSide");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Feil brukernavn eller passord");
            return View("LoggInn");
        }
    }
    
    public IActionResult MidlertidigBypass()
    {
        return RedirectToAction("LandingsSide", "LandingsSide");
    }
    public IActionResult VisLoggInnSide()
    {
        return View("LoggInn");
    }
}

