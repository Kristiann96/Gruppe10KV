using AuthInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    [Authorize(Roles = UserRoles.Saksbehandler)]
    public class LandingsSideSaksBController : Controller
    {
        public IActionResult LandingsSideSaksB()
        {
            return View();
        }
    }
}
