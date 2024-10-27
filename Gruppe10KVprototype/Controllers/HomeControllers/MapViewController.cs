using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.HomeControllers
{
    public class MapViewController : Controller
    {
        public IActionResult MapView()
        {
            return View();
        }
    }
}
