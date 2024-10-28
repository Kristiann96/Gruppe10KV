using Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using ViewModels;



namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class OversiktAlleInnmeldingerSaksBIncidentFormController : Controller
    {
        private readonly IIncidentFormRepository _repository;

        public OversiktAlleInnmeldingerSaksBIncidentFormController(IIncidentFormRepository repository)
        {
            _repository = repository;
        }



        // Viser en oversikt over alle oppføringer i incident_form tabellen


        public async Task<IActionResult> OversiktAlleInnmeldingerSaksBIncidentForm()
        {
            var forms = await _repository.GetAllAdviserFormsAsync();
            return View("OversiktAlleInnmeldingerSaksBIncidentForm", forms);
        }
    }
}
