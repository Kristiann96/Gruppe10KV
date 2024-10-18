using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.SaksbehandlerModels;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class SaksbehandlerInnmeldingController : Controller
    {
        private readonly ISaksbehandlerRepository _repository;

        public SaksbehandlerInnmeldingController(ISaksbehandlerRepository repository)
        {
            _repository = repository;
        }

        // Viser en oversikt over alle oppføringer i incident_form tabellen 
        public async Task<IActionResult> Index()
        {
            var forms = await _repository.GetAllAdviserFormsAsync();
            return View("SaksbehandlerInnmeldingOversiktView", forms);  
        }

        // Viser en enkelt oppføring av incident_form basert på ID 
        public async Task<IActionResult> SaksbehandlerSingleInnmeldingView(int id)
        {
            var form = await _repository.GetAdviserFormByIdAsync(id);
            if (form == null) return NotFound();
            return View(form);
        }

     

        // GET: Viser en tom InnmeldingSaksbehandlerView med en ny modell og statusliste
        [HttpGet("InnmeldingSaksbehandlerView")]
        public IActionResult InnmeldingSaksbehandlerView()
        {
            var model = new SaksbehandlerSingelInnmeldingViewModel
            {
                Innmelding = new SaksbehandlerINNMELDINGModel(), // Initierer ny modell
                StatusList = GetStatusList() // Henter liste over tilgjengelige statuser
            };
            return View(model);  // Viser viewet med tom modell og statusliste
        }

        // POST: Oppdater viewet basert på innsendt data
        [HttpPost("InnmeldingSaksbehandlerView")]
        public async Task<IActionResult> InnmeldingSaksbehandlerView(SaksbehandlerSingelInnmeldingViewModel model)
        {
            if (model == null) // Hvis modellen er null, lag en ny tom modell
            {
                model = new SaksbehandlerSingelInnmeldingViewModel();
            }

            model.StatusList = GetStatusList(); // Fyll dropdown med statuser

            if (model.Innmelding != null && model.Innmelding.InnmeldID > 0)  // Hvis InnmeldID er gyldig
            {
                var innmelding = await _repository.GetInnmeldingByIdAsync(model.Innmelding.InnmeldID);  // Henter innmelding basert på ID
                if (innmelding != null)
                {
                    model.Innmelding = innmelding;  // Oppdaterer modellen med innmeldingsdata fra databasen
                }
            }

            return View(model);  // Returnerer viewet med oppdatert modell
        }

        // POST: Oppdater statusen på en sak i INNMELDING-tabellen
        [HttpPost]
        public async Task<IActionResult> UpdateStatusAsync(SaksbehandlerSingelInnmeldingViewModel model)
        {
            if (ModelState.IsValid) // Validerer input fra brukeren
            {
                var success = await _repository.UpdateStatusAsync(model.Innmelding.InnmeldID, model.Innmelding.StatusID); // Oppdaterer status

                if (success)
                {
                    // Oppdaterer dropdown med tilgjengelige statuser etter vellykket statusendring
                    model.StatusList = GetStatusList();
                    return View("InnmeldingSaksbehandlerView", model); // Oppdater viewet med ny status
                }
                else
                {
                    ModelState.AddModelError("", "Kunne ikke oppdatere status.");
                }
            }

            return View("InnmeldingSaksbehandlerView", model); // Returnerer samme view ved valideringsfeil
        }

        // Henter tilgjengelige statuser for dropdown
        private List<SelectListItem> GetStatusList()
        {
            return _repository.GetAvailableStatuses().Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).ToList();
        }
    }
}
