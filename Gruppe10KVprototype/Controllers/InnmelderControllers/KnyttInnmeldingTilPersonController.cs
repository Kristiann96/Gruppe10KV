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
    [AutoValidateAntiforgeryToken]
    public class KnyttInnmeldingTilPersonController : Controller
    {
        private readonly IInnmeldingLogic _innmeldingLogic;

        public KnyttInnmeldingTilPersonController(IInnmeldingLogic innmeldingLogic)
        {
            _innmeldingLogic = innmeldingLogic;
        }

        [HttpGet]
        public async Task<IActionResult> KnyttInnmeldingTilPerson(KnyttInnmeldingTilPersonViewModel model)
        {
            
            if (string.IsNullOrEmpty(model.GeometriGeoJson) ||
                string.IsNullOrEmpty(model.Tittel) ||
                string.IsNullOrEmpty(model.Beskrivelse))
            {
                return RedirectToAction("KartfeilMarkering", "KartfeilMarkering");
            }
            bool innlogget = !string.IsNullOrEmpty(User.Identity?.Name);
            if (innlogget)
            {
                ModelState.Remove("Epost");
                model.Epost = User.Identity?.Name;
                return await LagreKnyttInnmeldingTilPerson(model);
            }
            return View(model);
        }

        [HttpPost]
       
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

                if (!resultat)
                {
                    ModelState.AddModelError("", "Kunne ikke lagre innmeldingen. Vennligst prøv igjen.");
                    return View("KnyttInnmeldingTilPerson", model);
                }
                return innlogget
                    ? RedirectToAction("LandingsSide", "LandingsSide")
                    : RedirectToAction("Index", "Home");
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
