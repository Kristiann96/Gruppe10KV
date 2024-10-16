using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers;

public class LandingPageController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}