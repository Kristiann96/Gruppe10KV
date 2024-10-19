using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;

public class Innmelder : Controller
{
    public IActionResult InnmeldingMineSaker()
    {
        return View();
    }
}