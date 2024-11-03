using LogicInterfaces;
using Logic;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{



    public class KnyttInnmeldingTilPersonController : Controller
    {
        private readonly IInnmeldingOpprettelseLogic _innmeldingOpprettelseLogic;

        public KnyttInnmeldingTilPersonController(IInnmeldingOpprettelseLogic innmeldingOpprettelseLogic)
        {
            _innmeldingOpprettelseLogic = innmeldingOpprettelseLogic;
        }

        [HttpGet]
        public IActionResult KnyttInnmeldingTilPerson(KnyttInnmeldingTilPersonViewModel model)
        {
            // Verifiser at vi har nødvendig data
            if (string.IsNullOrEmpty(model.GeometriGeoJson) ||
                string.IsNullOrEmpty(model.Tittel) ||
                string.IsNullOrEmpty(model.Beskrivelse))
            {
                return RedirectToAction("KartfeilMarkering", "KartfeilMarkering");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LagreKnyttInnmeldingTilPerson(KnyttInnmeldingTilPersonViewModel model)
        {
            System.Diagnostics.Debug.WriteLine("=== POST ACTION TRIGGERED ===");

            try
            {
                System.Diagnostics.Debug.WriteLine($"Email: {model.Epost}");
                System.Diagnostics.Debug.WriteLine($"Tittel: {model.Tittel}");
                System.Diagnostics.Debug.WriteLine($"Beskrivelse: {model.Beskrivelse}");
                System.Diagnostics.Debug.WriteLine($"ErNodEtatKritisk: {model.ErNodEtatKritisk}");
                System.Diagnostics.Debug.WriteLine($"GeoJson exists: {!string.IsNullOrEmpty(model.GeometriGeoJson)}");
                System.Diagnostics.Debug.WriteLine($"Raw GeoJson: {model.GeometriGeoJson}");

                if (!ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("ModelState is invalid:");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        System.Diagnostics.Debug.WriteLine($"Error: {error.ErrorMessage}");
                    }
                    return View(model);
                }

                var gjesteinnmelder = new GjesteinnmelderModel
                {
                    Epost = model.Epost
                };

                var innmelding = new InnmeldingModel
                {
                    Tittel = model.Tittel,
                    Beskrivelse = model.Beskrivelse,
                    Prioritet = model.ErNodEtatKritisk ? "høy" : "ikke_vurdert"
                };

                var geometri = new Geometri
                {
                    GeometriGeoJson = model.GeometriGeoJson
                };

                System.Diagnostics.Debug.WriteLine("=== About to call ValidereOgLagreNyInnmelding ===");

                var resultat = await _innmeldingOpprettelseLogic.ValidereOgLagreNyInnmelding(
                    innmelding,
                    geometri,
                    model.Epost);

                System.Diagnostics.Debug.WriteLine($"=== Result from ValidereOgLagreNyInnmelding: {resultat} ===");

                if (resultat)
                {
                    return RedirectToAction("LandingsSide", "LandingsSide");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== ERROR: {ex.GetType().Name} ===");
                System.Diagnostics.Debug.WriteLine($"=== Message: {ex.Message} ===");
                System.Diagnostics.Debug.WriteLine($"=== StackTrace: {ex.StackTrace} ===");

                ModelState.AddModelError("", "Det oppstod en feil ved innsending. Vennligst prøv igjen.");
            }

            return View("KnyttInnmeldingTilPerson", model);
        }

       
    }
}
