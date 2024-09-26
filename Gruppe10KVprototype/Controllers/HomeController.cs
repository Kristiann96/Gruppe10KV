using Gruppe10KVprototype.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Gruppe10KVprototype.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult MapView()
        {
            return View();
        }
        public IActionResult Form()
         {
        return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //Sjekker verdiene i Form
        [HttpPost]
        public IActionResult SubmitForm(IncidentFormModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the form data
                return View("FormResult", model);
            }
            return View(model);
        }
        //Viser resultatet av Form
        public IActionResult FormResult(IncidentFormModel model)
        {
            return View(model);
        }
    }
}
