using Interface;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using LogicInterfaces;
using Models.Models;

public class KartfeilMarkeringController : Controller
{
    private readonly IGeometriRepository _geometriRepository;
    private readonly IKommuneAPILogic _kommuneAPILogic;

    public KartfeilMarkeringController(
        IGeometriRepository geometriRepository,
        IKommuneAPILogic kommuneAPILogic)
    {
        _geometriRepository = geometriRepository;
        _kommuneAPILogic = kommuneAPILogic;
    }

    public IActionResult KartfeilMarkering()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult GaaTilSkjema([FromBody] Geometri geometri)
    {
        if (string.IsNullOrEmpty(geometri.GeometriGeoJson))
        {
            return BadRequest("Ingen geometri er tegnet på kartet");
        }

        // Validerer at det er gyldig GeoJSON
        try
        {
            JsonDocument.Parse(geometri.GeometriGeoJson);
        }
        catch
        {
            return BadRequest("Ugyldig geometri-data");
        }

        return RedirectToAction("KartfeilSkjema", "KartfeilSkjema",
            new { geoJson = geometri.GeometriGeoJson });
    }
}