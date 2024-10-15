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

    public IActionResult SingleCaseView()
    {
        return View();
    }


    public IActionResult OversiktSakerSaksbehandler()
    {
        return View();
    }

    public IActionResult InnloggingSaksbehandler()
    {
        return View();
    }

    public IActionResult SortereSaker()
    {
        return View();
    }


    public IActionResult SearchForInnmelder()
    {
        return View();
    }

}