using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        List<string> statuser = new List<string>()
        {
            "Ikke tatt til følge",
            "Under behandling",
            "Ferdig behandlet"
        };

        ViewBag.Statuser = SelectList(statuser);
        
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