using Interface;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class KartvisningEnInnmeldingSaksBController : Controller
    {
        private readonly IGeometriRepository _geometriRepository;
        private readonly IDataSammenstillingSaksBRepository _dataSammenstillingsRepo;

        public KartvisningEnInnmeldingSaksBController(
            IGeometriRepository geometriRepository,
            IDataSammenstillingSaksBRepository dataSammenstillingsRepo)
        {
            _geometriRepository = geometriRepository;
            _dataSammenstillingsRepo = dataSammenstillingsRepo;
        }

        [HttpGet]
        public async Task<IActionResult> KartvisningEnInnmeldingSaksB(int innmeldingId)
        {
            // Hent sammenstilt data
            var (innmelding, person, innmelder, saksbehandler) =
                await _dataSammenstillingsRepo.GetInnmeldingMedDetaljerAsync(innmeldingId);

            if (innmelding == null)
            {
                return NotFound("Innmelding ikke funnet");
            }

            // Hent geometri data
            var geometriData = await _geometriRepository.GetGeometriByInnmeldingIdAsync(innmeldingId);

            var viewModel = new KartvisningEnInnmeldingSaksBViewModel
            {
                Innmelding = innmelding,
                Person = person,
                Innmelder = innmelder,
                Saksbehandler = saksbehandler,
                GeometriData = geometriData
            };

            

            return View(viewModel);
        }
    }
}