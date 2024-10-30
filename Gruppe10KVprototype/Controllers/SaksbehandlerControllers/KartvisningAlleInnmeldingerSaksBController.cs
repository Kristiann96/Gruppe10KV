using Microsoft.AspNetCore.Mvc;
using Interface;
using Logic;
using Models.Models;
using ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class KartvisningAlleInnmeldingerSaksBController : Controller
    {
        private readonly IGeometriRepository _geometriRepository;
        private readonly IKartverketAPILogic _kartverketAPILogic;

        public KartvisningAlleInnmeldingerSaksBController(IGeometriRepository geometriRepository, IKartverketAPILogic kartverketAPILogic)
        {
            _geometriRepository = geometriRepository;
            _kartverketAPILogic = kartverketAPILogic;
        }

        // Henter alle geometriobjekter og kommunedata fra Kartverket og sender dem til viewet
        [HttpGet]
        public async Task<IActionResult> KartvisningAlleInnmeldingerSaksB()
        {
            // Hent data fra repository og API
            IEnumerable<Geometri> geometriData = await _geometriRepository.GetAllGeometriAsync();
            var kommunerData = await _kartverketAPILogic.GetKommunerAsync();

            // Opprett ViewModel og sett data
            var viewModel = new KartvisningAlleInnmeldingerSaksBViewModel
            {
                GeometriData = geometriData,
                KommunerData = kommunerData
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetKommunenummer(int innmeldingId, double lat, double lng)
        {
            var kommune = await _kartverketAPILogic.GetKommuneByCoordinatesAsync(lat, lng);

            TempData["Kommunenummer"] = kommune.Kommunenummer;
            TempData["Kommunenavn"] = kommune.Kommunenavn;

            return RedirectToAction("KartvisningEnInnmeldingSaksB", "KartvisningEnInnmeldingSaksB", new { innmeldingId });
        }
    }
}


