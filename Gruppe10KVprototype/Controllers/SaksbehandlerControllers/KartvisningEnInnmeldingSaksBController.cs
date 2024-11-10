using Interface;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models;
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
        public async Task<IActionResult> KartvisningEnInnmeldingSaksB(int? innmeldingId, string innmeldingIds)
        {
            // Liste som skal holde alle innmeldinger med detaljer
            var alleSaker = new List<(InnmeldingModel, PersonModel, InnmelderModel, SaksbehandlerModel, Geometri)>();

            if (innmeldingId.HasValue)
            {
                // Håndter enkelt innmelding som før
                var (innmelding, person, innmelder, saksbehandler) =
                    await _dataSammenstillingsRepo.GetInnmeldingMedDetaljerAsync(innmeldingId.Value);
                var geometriData = await _geometriRepository.GetGeometriByInnmeldingIdAsync(innmeldingId.Value);

                if (innmelding != null)
                {
                    alleSaker.Add((innmelding, person, innmelder, saksbehandler, geometriData));
                }
            }
            else if (!string.IsNullOrEmpty(innmeldingIds))
            {
                // Håndter flere innmeldinger
                var idListe = innmeldingIds.Split(',').Select(int.Parse);

                foreach (var id in idListe)
                {
                    var (innmelding, person, innmelder, saksbehandler) =
                        await _dataSammenstillingsRepo.GetInnmeldingMedDetaljerAsync(id);
                    var geometriData = await _geometriRepository.GetGeometriByInnmeldingIdAsync(id);

                    if (innmelding != null)
                    {
                        alleSaker.Add((innmelding, person, innmelder, saksbehandler, geometriData));
                    }
                }
            }

            if (!alleSaker.Any())
            {
                return NotFound("Ingen innmeldinger funnet");
            }

            var viewModel = new KartvisningEnInnmeldingSaksBViewModel
            {
                AlleInnmeldinger = alleSaker
            };

            return View(viewModel);
        }
    }
}