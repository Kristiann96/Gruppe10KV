using AuthInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.HomeViewModels;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    [Authorize(Roles = UserRoles.Innmelder)]
    [AutoValidateAntiforgeryToken]
    public class LandingsSideController : Controller
    {
        public IActionResult LandingsSide()
        {
            var model = new SuccessMeldingViewModel()
            { 
                Message = TempData["SuccessMessage"] as string 
            };
            return View(model);
        }
    }
}
