using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;

public class InnmelderOppdatereInnmeldingController : Controller
{
    public IActionResult InnmelderOppdatereInnmelding()
    {
        return View(InnmelderOppdatereInnmelding);
    }
}