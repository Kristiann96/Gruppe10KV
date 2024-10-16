using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers;

public class SaksbehandlerController : Controller
{
    // GET
    public IActionResult LandingPage()
    {
        return View();
    }

    public IActionResult InnmeldingSaksbehandlerView()
    {
        return View();
    }


    public IActionResult OversiktInnmeldingerSaksbehandler()
    {
        return View();
    }

    public IActionResult InnloggingSaksbehandler()
    {
        return View();
    }

    public IActionResult SortereInnmeldinger()
    {
        return View();
    }


    public IActionResult SøkInnmelderSaksbehandler()
    {
        return View();
    }
}