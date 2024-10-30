using Interface;
using Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class KartvisningEnInnmeldingSaksBController : Controller
    {
        private readonly IInnmeldingRepository _innmeldingRepository;
        private readonly IGeometriRepository _geometriRepository;
        private readonly IInnmeldingEnumLogic _innmeldingEnumLogic;

        public KartvisningEnInnmeldingSaksBController(IInnmeldingRepository innmeldingRepository,
            IGeometriRepository geometriRepository,
            IInnmeldingEnumLogic innmeldingEnumLogic)
        {
            _innmeldingRepository = innmeldingRepository;
            _geometriRepository = geometriRepository;
            _innmeldingEnumLogic = innmeldingEnumLogic;
        }

        [HttpGet]
        public async Task<IActionResult> KartvisningEnInnmeldingSaksB(int innmeldingId)
        {
            var innmeldingDetaljer = await _innmeldingRepository.GetInnmeldingDetaljerByIdAsync(innmeldingId);
            var geometriData = await _geometriRepository.GetGeometriByInnmeldingIdAsync(innmeldingId);
            var statusOptions = await _innmeldingEnumLogic.GetFormattedStatusEnumValuesAsync();


            var viewModel = new KartvisningEnInnmeldingSaksBViewModel
            {
                InnmeldingDetaljer = innmeldingDetaljer,
                GeometriData = geometriData,
                StatusOptions = new SelectList(statusOptions)
            };

            return View(viewModel);
        }
    }
}
