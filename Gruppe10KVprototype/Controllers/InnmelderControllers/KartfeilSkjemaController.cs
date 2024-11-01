using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    public class KartfeilSkjemaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
//her kommer refactorert InnmelderSkjemaIncidentFormController - denne er en kontroller for 2 stk views pr nå