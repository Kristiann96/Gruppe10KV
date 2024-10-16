using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers;

public class TempSaksbehandlerController : Controller
{
    // GET
    public IActionResult LandingPage()
    {
        return View();
    }

    public IActionResult SingleCaseView()
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


    public IActionResult SearchForInnmelder()
    {
        return View();
    }
}