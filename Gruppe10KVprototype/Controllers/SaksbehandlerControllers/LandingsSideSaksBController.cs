using AuthInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    [Authorize(Roles = UserRoles.Saksbehandler)]
    [AutoValidateAntiforgeryToken]
    public class LandingsSideSaksBController : Controller
    {
        public IActionResult LandingsSideSaksB()
        {
            return View();
        }
    }
}
