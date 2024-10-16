using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Models;

public class LandingPageModel : Controller
{
    public IActionResult LandingPage()
    {
        return View();
    }
}