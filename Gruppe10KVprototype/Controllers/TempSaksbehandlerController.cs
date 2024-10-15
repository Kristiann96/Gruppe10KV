using Models.SaksbehandlerModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gruppe10KVprototype.Controllers;

public class TempSaksbehandlerController : Controller
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

    [HttpPost]
    public IActionResult SetStatus(Statuser status)
    {
        // Here you would typically update your database or perform any necessary logic
        // For this example, we'll just return a success status
        return Json(new { success = true, message = "Status updated successfully" });
    }

}