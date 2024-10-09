using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers
{
    public class UserPagesController : Controller
    {
        public IActionResult UserCases()
        {
            return View();
        }
    }
}
