using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;

public class FormUpdateController : Controller
{
    public IActionResult FormUpdate()
    {
        return View("~/Views/InnmelderSkjema/FormUpdate.cshtml");
    }
}