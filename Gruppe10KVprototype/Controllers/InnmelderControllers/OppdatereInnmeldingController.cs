using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using System.Threading.Tasks;
using Interface;
using System.Collections.Generic;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    public class OppdatereInnmeldingController : Controller
    {
        private readonly IInnmeldingRepository _innmeldingRepository;
        private readonly IGeometriRepository _geometriRepository;

        public ActionResult Index()
        {
            var viewModel = new OppdatereInnmeldingViewModel
            {
                OppdatereInnmeldinger = _innmeldingRepository.GetInnmeldingAsync()
            };
            return View(viewModel); 
        }

        public OppdatereInnmeldingController(IInnmeldingRepository repository, IGeometriRepository geometriRepository)
        {
            _innmeldingRepository = repository;
            _geometriRepository = geometriRepository;
        }
        
        public async Task<IActionResult> OppdatereInnmelding()
        {
            // Retrieve data from repository
            IEnumerable<InnmeldingModel> innmeldinger = await _innmeldingRepository.GetInnmeldingAsync();

            // Pass data to the view
            return View(innmeldinger);
        }
    }
}
