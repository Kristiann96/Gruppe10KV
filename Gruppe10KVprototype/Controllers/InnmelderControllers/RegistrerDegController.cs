using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    public class RegistrerDegController : Controller
    {
        public IActionResult RegistrerDeg()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegistrerInnmelder(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("RegistrerDeg");
        }
    }
}
