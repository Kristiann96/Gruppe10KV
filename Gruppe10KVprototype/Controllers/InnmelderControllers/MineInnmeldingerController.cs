using Interface;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gruppe10KVprototype.Controllers
{
    public class MineInnmeldingerController : Controller
    {
        private readonly IInnmeldingRepository _innmeldingRepository;

        public MineInnmeldingerController(IInnmeldingRepository innmeldingRepository)
        {
            _innmeldingRepository = innmeldingRepository;
        }

        public async Task<IActionResult> MineInnmeldinger()
        {
            // Bruk en hardkodet innmelderId for testing
            int innmelderId = 101; // Endre dette til en gyldig ID for testing

            // Hent innmeldinger for den aktuelle innmelderen
            IEnumerable<InnmeldingModel> innmeldinger = await _innmeldingRepository.GetInnmeldingerAvInnmelderIdAsync(innmelderId);

            // Send innmelder_id til viewet
            ViewBag.InnmelderId = innmelderId;

            // Returner visningen med innmeldinger
            return View(innmeldinger);
        }
    }
}
