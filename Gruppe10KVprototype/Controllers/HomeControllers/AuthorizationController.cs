using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.HomeViewModels;

namespace Gruppe10KVprototype.Controllers.HomeControllers;

public class AuthorizationController : Controller
{
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(ILogger<AuthorizationController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        _logger.LogInformation("AccessDenied action was called");
        var errorMessage = "Du har ikke adgang til denne siden";
        TempData["ErrorMessage"] = errorMessage;
        return RedirectToAction( "Error", "Home");

    }
}
