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

       

        [HttpGet("InnmeldingSaksbehandlerView")]
        public IActionResult InnmeldingSaksbehandlerView()
        {
            var model = new SaksbehandlerSingelInnmeldingViewModel
            {
                Innmelding = new SaksbehandlerINNMELDINGModel(),
                StatusList = GetStatusList()
            };
            return View(model);
        }

        [HttpPost("InnmeldingSaksbehandlerView")]
        public async Task<IActionResult> InnmeldingSaksbehandlerView(SaksbehandlerSingelInnmeldingViewModel model)
        {
            if (model == null)
            {
                model = new SaksbehandlerSingelInnmeldingViewModel();
            }
            model.StatusList = GetStatusList();
            if (model.Innmelding != null && model.Innmelding.InnmeldID > 0)
            {
                var innmelding = await _repository.GetInnmeldingByIdAsync(model.Innmelding.InnmeldID);
                if (innmelding != null)
                {
                    model.Innmelding = innmelding;
                }
            }
            return View(model);
        }

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
