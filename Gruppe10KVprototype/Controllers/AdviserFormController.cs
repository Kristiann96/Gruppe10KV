using Microsoft.AspNetCore.Mvc;
using Interfaces;
using Entities;
using System.Threading.Tasks;

namespace Gruppe10KVprototype.Controllers
{
    public class AdviserFormController : Controller
    {
        private readonly IAdviserFormRepository _repository;

        public AdviserFormController(IAdviserFormRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var forms = await _repository.GetAllAdviserFormsAsync();
            return View("AdviserFormView", forms);
        }

        public async Task<IActionResult> AdviserSingleFormView(int id)
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