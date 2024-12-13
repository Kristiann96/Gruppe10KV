using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.HomeViewModels;

namespace Gruppe10KVprototype.Controllers.HomeControllers
{
    [AllowAnonymous]
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Saksbehandler"))
                {
                    return RedirectToAction("LandingsSideSaksB", "LandingsSideSaksB");
                }

                return RedirectToAction("LandingsSide", "LandingsSide");
            }
            
            var model = new SuccessMeldingViewModel 
            { 
                Message = TempData["SuccessMessage"] as string 
            };
            return View(model);
        }
            
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorMessage = TempData["ErrorMessage"] as string;
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = errorMessage
            });
        }
    }
}       
    
