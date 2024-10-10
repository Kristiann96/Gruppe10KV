using Gruppe10KVprototype.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Models;
using Models.InnmeldingModels;

namespace Gruppe10KVprototype.Controllers
{
    public class IncidentFormController : Controller
    {
        private readonly IncidentFormDBContext _dbContext;

        public IncidentFormController(IncidentFormDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Viser skjemaet
        public IActionResult Form(string geoJson)
        {
            var model = new IncidentFormModel { GeoJson = geoJson };
            return View("Form");
        }

        // Tar imot skjemaet og lagrer det i databasen
        [HttpPost]
        public async Task<IActionResult> SubmitForm(IncidentFormModel model)
        {
            if (ModelState.IsValid)
            {
                await _dbContext.SaveIncidentForm(model);  // Lagrer skjemaet i databasen
                return View("FormResult", model);          // Viser resultatet
            }
            return View("Form", model);  // Viser skjemaet igjen hvis validering feiler
        }

        // Viser resultatet etter skjemaet er sendt inn
        public IActionResult FormResult(IncidentFormModel model)
        {
            return View(model);
        }
    }
}
