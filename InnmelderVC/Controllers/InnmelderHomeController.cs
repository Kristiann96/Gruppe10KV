using Microsoft.AspNetCore.Mvc;

namespace InnmelderVC.Controllers
{
    public class InnmelderHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
