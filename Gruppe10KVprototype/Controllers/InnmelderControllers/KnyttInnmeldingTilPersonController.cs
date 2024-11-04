using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Exceptions;
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("KnyttInnmeldingTilPerson", model);
                }

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

                var resultat = await _innmeldingOpprettelseLogic.ValidereOgLagreNyInnmelding(
                    innmelding,
                    geometri,
                    model.Epost);

                if (resultat)
                {
                    return RedirectToAction("LandingsSide", "LandingsSide");
                }

                ModelState.AddModelError("", "Kunne ikke lagre innmeldingen. Vennligst prøv igjen.");
                return View("KnyttInnmeldingTilPerson", model);
            }
            catch (ForretningsRegelExceptionModel ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("KnyttInnmeldingTilPerson", model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Det oppstod en uventet feil. Vennligst prøv igjen.");
                return View("KnyttInnmeldingTilPerson", model);
            }
        }
    }
}
