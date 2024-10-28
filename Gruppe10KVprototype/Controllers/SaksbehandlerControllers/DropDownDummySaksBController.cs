
using Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;

using ViewModels;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class DropDownDummySaksBController : Controller
    {
        private readonly IInnmeldingERepository _repository;

        public DropDownDummySaksBController(IInnmeldingERepository repository)
        {
            _repository = repository;
        }

        //INNMELDIGSTABELLEN
        // GET: // Initialiserer en tom ViewModel med statusliste
        [HttpGet("DropDownDummySaksB")]
        public IActionResult DropDownDummySaksB()
        {
            var model = new DropDownDummySaksBViewModel
            {
                InnmeldingE = new InnmeldingEModel(), // Initierer ny modell
                StatusList = GetStatusList() // Henter liste over tilgjengelige statuser
            };
            return View(model);  // Viser viewet med tom modell og statusliste
        }

        // POST: Fyller ViewModel med data fra databasen basert på InnmeldID
        [HttpPost("DropDownDummySaksB")]
        public async Task<IActionResult> DropDownDummySaksB(DropDownDummySaksBViewModel model)
        {
            if (model == null) // Hvis modellen er null, lag en ny tom modell
            {
                model = new DropDownDummySaksBViewModel();
            }

            model.StatusList = GetStatusList(); // Fyll dropdown med statuser

            if (model.InnmeldingE != null && model.InnmeldingE.InnmeldID > 0)  // Hvis InnmeldID er gyldig
            {
                var innmelding = await _repository.GetInnmeldingByIdAsync(model.InnmeldingE.InnmeldID);  // Henter innmeldingE basert på ID
                if (innmelding != null)
                {
                    model.InnmeldingE = innmelding;  // Oppdaterer modellen med innmeldingsdata fra databasen
                }
            }

            return View(model);  // Returnerer viewet med oppdatert modell
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatusAsync(DropDownDummySaksBViewModel model)
        {
            // Oppdaterer status i databasen og returnerer oppdatert ViewModel
            var success = await _repository.UpdateStatusAsync(model.InnmeldingE.InnmeldID, model.InnmeldingE.StatusID);
            if (success)
            {
                // Hent oppdatert innmelding
                model.InnmeldingE = await _repository.GetInnmeldingByIdAsync(model.InnmeldingE.InnmeldID);
            }
            model.StatusList = GetStatusList();
            return View("DropDownDummySaksB", model);
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
