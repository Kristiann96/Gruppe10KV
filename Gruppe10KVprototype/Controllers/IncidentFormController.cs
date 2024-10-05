using Gruppe10KVprototype.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Gruppe10KVprototype.Controllers
{
    public class IncidentFormController : Controller
    {
        private readonly MariaDbContext _dbContext;

        public IncidentFormController(MariaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Viser skjemaet
        public IActionResult Form(string geoJson)
        {
            // Setter geoJson-dataen som ViewBag for å sende til skjemaet
            ViewBag.geoJson = geoJson;
            var model = new IncidentFormModel { GeoJson = geoJson };  // Setter GeoJson i modellen
            return View("Form");
        }

        // Tar imot skjemaet og lagrer det i databasen
        [HttpPost]
        public async Task<IActionResult> SubmitForm(IncidentFormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _dbContext.SaveIncidentForm(model);  // Forsøk å lagre i databasen
                }
                catch (Exception)
                {
                    // Logg feilen her om nødvendig
                    ModelState.AddModelError("", "Kunne ikke lagre dataene, databasen er nede. Vi viser deg likevel skjemaet.");
                }

                return View("FormResult", model);  // Viser FormResult selv om det er en databasefeil
            }
            return View("Form", model);  // Gå tilbake til skjemaet hvis validering feiler
        }

        // Viser resultatet etter skjemaet er sendt inn
        public IActionResult FormResult(IncidentFormModel model)
        {
            return View(model);
        }
    }
}
