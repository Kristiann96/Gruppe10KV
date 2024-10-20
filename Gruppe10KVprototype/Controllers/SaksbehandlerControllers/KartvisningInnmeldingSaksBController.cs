using Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;

using ViewModels;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class KartvisningInnmeldingSaksBController : Controller
    {
        private readonly IIncidentFormRepository _repository;

        public KartvisningInnmeldingSaksBController(IIncidentFormRepository repository)
        {
            _repository = repository;
        }



        // Viser en enkelt oppføring av incident_form basert på ID
        public async Task<IActionResult> KartvisningInnmeldingSaksB(int id)
        {
            var form = await _repository.GetAdviserFormByIdAsync(id);
            if (form == null) return NotFound();
            return View(form);
        }
    }
}