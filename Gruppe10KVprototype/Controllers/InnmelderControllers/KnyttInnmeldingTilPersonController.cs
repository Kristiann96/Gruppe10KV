using AuthInterface;
using LogicInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Exceptions;
using Models.Models;
using System.Data;
using ViewModels;


namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    public class KnyttInnmeldingTilPersonController : Controller
    {
        private readonly IInnmeldingLogic _innmeldingLogic;

        public KnyttInnmeldingTilPersonController(IInnmeldingLogic innmeldingLogic)
        {
            _innmeldingLogic = innmeldingLogic;
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
            bool innlogget = !string.IsNullOrEmpty(User.Identity?.Name);
            
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

                var resultat = await _innmeldingLogic.ValidereOgLagreNyInnmelding(
                    innmelding,
                    geometri,
                    model.Epost,
                    innlogget);

                if (resultat || !innlogget)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (resultat || innlogget)
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
