using Microsoft.AspNetCore.Mvc;
using Interface;
using Logic;
using Models.Models;
using ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogicInterfaces;
using AuthInterface;
using Microsoft.AspNetCore.Authorization;


namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    [Authorize(Roles = UserRoles.Saksbehandler)]
    [AutoValidateAntiforgeryToken]
    public class KartvisningAlleInnmeldingerSaksBController : Controller
    {
        private readonly IGeometriRepository _geometriRepository;
        private readonly IKommuneAPILogic _kommuneAPILogic;

        public KartvisningAlleInnmeldingerSaksBController(IGeometriRepository geometriRepository, IKommuneAPILogic kommuneAPILogic)
        {
            _geometriRepository = geometriRepository;
            _kommuneAPILogic = kommuneAPILogic;
        }

        
        [HttpGet]
        public async Task<IActionResult> KartvisningAlleInnmeldingerSaksB()
        {
            // Hent data fra repository og API
            IEnumerable<Geometri> geometriData = await _geometriRepository.GetAllGeometriAsync();
            var kommunerData = await _kommuneAPILogic.GetKommunerAsync();

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
            var kommune = await _kommuneAPILogic.GetKommuneByCoordinatesAsync(lat, lng);

            TempData["Kommunenummer"] = kommune.Kommunenummer;
            TempData["Kommunenavn"] = kommune.Kommunenavn;

            return RedirectToAction("KartvisningEnEllerFlereInnmeldingSaksB", "KartvisningEnEllerFlereInnmeldingSaksB", new { innmeldingId });
        }
    }
}


