using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    public class LandingsSideController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
