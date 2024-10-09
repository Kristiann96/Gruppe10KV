using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Models
{
    public class LoginModel : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
