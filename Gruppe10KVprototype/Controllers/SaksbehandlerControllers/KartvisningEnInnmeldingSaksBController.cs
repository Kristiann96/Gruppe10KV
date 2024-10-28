using Interface;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class KartvisningEnInnmeldingSaksBController : Controller
    {
        private readonly IInnmeldingRepository _innmeldingRepository;
        private readonly IGeometriRepository _geometriRepository;

        public KartvisningEnInnmeldingSaksBController(IInnmeldingRepository innmeldingRepository,
            IGeometriRepository geometriRepository)
        {
            _innmeldingRepository = innmeldingRepository;
            _geometriRepository = geometriRepository;
        }

        [HttpGet]
        public async Task<IActionResult> KartvisningEnInnmeldingSaksB(int innmeldingId)
        {
            var innmeldingDetaljer = await _innmeldingRepository.GetInnmeldingDetaljerByIdAsync(innmeldingId);
            var geometriData = await _geometriRepository.GetGeometriByInnmeldingIdAsync(innmeldingId);

            var viewModel = new KartvisningEnInnmeldingSaksBViewModel
            {
                InnmeldingDetaljer = innmeldingDetaljer,
                GeometriData = geometriData
            };

            return View(viewModel);
        }
    }
}
