using Interface;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using System.Text.Json;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    [AutoValidateAntiforgeryToken]
    public class SporKartfeilController : Controller
    {
        private readonly IGeometriRepository _geometriRepository;
        private readonly IKommuneAPILogic _kommuneAPILogic;

        public SporKartfeilController(
            IGeometriRepository geometriRepository,
            IKommuneAPILogic kommuneAPILogic)
        {
            _geometriRepository = geometriRepository;
            _kommuneAPILogic = kommuneAPILogic;
        }

        public IActionResult SporKartfeil()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GaaTilSkjema([FromBody] Geometri geometri)
        {
            if (string.IsNullOrEmpty(geometri.GeometriGeoJson))
            {
                return BadRequest("Ingen sporet rute funnet");
            }

            try
            {
                JsonDocument.Parse(geometri.GeometriGeoJson);
            }
            catch
            {
                return BadRequest("Ugyldig sporingsdata");
            }

            return RedirectToAction("KartfeilSkjema", "KartfeilSkjema",
                new { geoJson = geometri.GeometriGeoJson });
        }
    }
}
