using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using System.Threading.Tasks;
using Interface;
using System.Collections.Generic;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    public class OppdatereInnmeldingController : Controller
    {
        private readonly IInnmeldingRepository _repository;

        public OppdatereInnmeldingController(IInnmeldingRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> OppdatereInnmelding()
        {
            // Retrieve data from repository
            IEnumerable<InnmeldingModel> innmeldinger = await _repository.GetInnmeldingAsync();

            // Pass data to the view
            return View(innmeldinger);
        }
    }
}
