using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class KartvisningInnmeldingSakBController : Controller
    {
        private readonly IIncidentFormRepository _repository;

        public KartvisningInnmeldingSakBControllerontroller(IIncidentFormRepository repository)
        {
            _repository = repository;
        }

       

        // Viser en enkelt oppføring av incident_form basert på ID
        public async Task<IActionResult> SaksbehandlerSingleInnmeldingView(int id)
        {
            var form = await _repository.GetAdviserFormByIdAsync(id);
            if (form == null) return NotFound();
            return View(form);
        }
    }
}