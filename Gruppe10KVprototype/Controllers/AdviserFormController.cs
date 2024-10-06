using Gruppe10KVprototype.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gruppe10KVprototype.Controllers
{
    public class AdviserFormController : Controller
    {
        private readonly MariaDbContext _dbContext;

        public AdviserFormController(MariaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Henter data fra databasen og sender til view
        public async Task<IActionResult> Index()
        {
            // Hent alle oppføringer fra incident_form
            List<IncidentFormModel> forms = await _dbContext.GetAllIncidents();

            // Send dataene til viewet
            return View("AdviserFormView", forms);
        }
    }
}
