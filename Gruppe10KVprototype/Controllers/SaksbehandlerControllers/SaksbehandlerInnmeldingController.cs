using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;

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


        //INNMELDIGSTABELLEN
        // GET: // Initialiserer en tom ViewModel med statusliste
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

        // POST: Fyller ViewModel med data fra databasen basert på InnmeldID
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

        [HttpPost]
        public async Task<IActionResult> UpdateStatusAsync(SaksbehandlerSingelInnmeldingViewModel model)
        {
            // Oppdaterer status i databasen og returnerer oppdatert ViewModel
            if (ModelState.IsValid)
            {
                var success = await _repository.UpdateStatusAsync(model.Innmelding.InnmeldID, model.Innmelding.StatusID);
                if (success)
                {
                    // Hent oppdatert innmelding
                    model.Innmelding = await _repository.GetInnmeldingByIdAsync(model.Innmelding.InnmeldID);
                }
            }
            model.StatusList = GetStatusList();
            return View("InnmeldingSaksbehandlerView", model);
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
