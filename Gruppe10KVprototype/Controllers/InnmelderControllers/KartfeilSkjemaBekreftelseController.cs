using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    public class KartfeilSkjemaBekreftelseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
//her kommer refactorert InnmelderSkjemaIncidentFormController - denne er pr idag en kontroller for 2 views