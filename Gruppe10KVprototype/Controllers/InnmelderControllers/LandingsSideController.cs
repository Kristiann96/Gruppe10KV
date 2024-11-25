using AuthInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    [Authorize(Roles = UserRoles.Innmelder)]
    public class LandingsSideController : Controller
    {
        public IActionResult LandingsSide()
        {
            return View();
        }
    }
}
