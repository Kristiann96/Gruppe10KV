using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class KartvisningInnmeldingSakBController : Controller
    {
        private readonly IIncidentFormRepository _repository;

        public KartvisningInnmeldingSakBController(IIncidentFormRepository repository)
        {
            _repository = repository;
        }
    }


// Viser en oversikt over alle oppføringer i incident_form tabellen


    public async Task<IActionResult> Index()
    {
        var forms = await _repository.GetAllAdviserFormsAsync();
        return View("KartvisningInnmeldingSakB", forms);
    }
}
