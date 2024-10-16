using Microsoft.AspNetCore.Mvc;
using Interfaces;
using Models;
using System.Threading.Tasks;

namespace Gruppe10KVprototype.Controllers
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
            if (form == null)
            {
                return NotFound();
            }
            return View(form);
        }

        
    }
}