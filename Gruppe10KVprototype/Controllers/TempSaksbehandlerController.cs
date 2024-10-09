using Gruppe10KVprototype.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers;

public class TempSaksbehandlerController : Controller
{
    // GET
    public IActionResult LandingPage()
    {
        return View();
    }
}