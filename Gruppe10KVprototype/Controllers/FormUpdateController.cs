using Gruppe10KVprototype.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers
{
    public class FormUpdateController : Controller
    {
        public IActionResult FormUpdate()
        {
            return View("~/Views/InnmelderSkjema/FormUpdate.cshtml");
        }
    }
}
