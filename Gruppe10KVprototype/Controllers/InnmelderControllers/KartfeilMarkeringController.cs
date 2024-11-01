using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    public class KartfeilMarkeringController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
//Her kommer refactorert MapViewcontroller