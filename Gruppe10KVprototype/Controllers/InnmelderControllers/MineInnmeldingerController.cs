using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    public class MineInnmeldingerController : Controller
    {
        public IActionResult MineInnmeldinger()
        {
            return View();
        }
    }
}
