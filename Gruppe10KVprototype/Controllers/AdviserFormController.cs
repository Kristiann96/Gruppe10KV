using Microsoft.AspNetCore.Mvc;
using Models.DbContexts;
using Models.SaksbehandlerModels;

namespace Gruppe10KVprototype.Controllers
{
    public class AdviserFormController : Controller
    {
        private readonly AdviserFormDBContext _dbContext;

        public AdviserFormController(AdviserFormDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Henter data fra databasen og sender til view
        public async Task<IActionResult> Index()
        {
            // Hent alle oppføringer fra incident_form
            List<AdviserFormModel> forms = await _dbContext.GetAllIncidents();

            // Send dataene til viewet
            return View("AdviserFormView", forms);
        }

        public async Task<IActionResult> AdviserSingleFormView(int id)
        {
            // Hent en enkelt sak fra databasen basert på id
            var incident = await _dbContext.GetIncidentById(id);

            if (incident == null)
            {
                return NotFound();
            }

            return View("AdviserSingleFormView", incident);  // Sender saken til viewet
        }

    }

}
