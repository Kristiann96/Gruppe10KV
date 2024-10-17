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

        public async Task<IActionResult> Index()
        {
            var forms = await _repository.GetAllAdviserFormsAsync();
            return View("SaksbehandlerInnmeldingOversiktView", forms);
        }

        public async Task<IActionResult> SaksbehandlerSingleInnmeldingView(int id)
        {
            var form = await _repository.GetAdviserFormByIdAsync(id);
            if (form == null) return NotFound();
            return View(form);
        }

        // Bruk av INNMELDING-tabell for å hente status og vise i dropdown
        [HttpGet]
        public async Task<IActionResult> GetInnmeldingStatus(int innmeldID)
        {
            var status = await _repository.GetStatusByInnmeldIdAsync(innmeldID);
            var viewModel = new SaksbehandlerSingelInnmeldingViewModel
            {
                InnmeldID = innmeldID,
                CurrentStatus = status,
                StatusList = _repository.GetAvailableStatuses().Select(s => new SelectListItem
                {
                    Text = s.ToString(),
                    Value = s.ToString(),
                    Selected = s == status
                }).ToList()
            };
            return View("InnmeldingSaksbehandlerView", viewModel); // Viser riktig view
        }

        // POST: Oppdaterer status for en innmelding basert på valgt status i dropdown
        [HttpPost]
        public async Task<IActionResult> EditStatus(SaksbehandlerSingelInnmeldingViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _repository.UpdateStatusAsync(model.InnmeldID, model.CurrentStatus);
                return RedirectToAction("GetInnmeldingStatus",
                    new { innmeldID = model.InnmeldID }); // Tilbake til samme side etter oppdatering
            }

            return
                View("InnmeldingSaksbehandlerView", model); // Hvis det er en feil, vis viewet igjen med feilmeldinger
        }
    }
}