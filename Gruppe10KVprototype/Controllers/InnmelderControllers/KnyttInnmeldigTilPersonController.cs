using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models;
using ViewModels;

public class KnyttInnmeldingTilPersonController : Controller
{
    private readonly IInnmeldingOpprettelseLogic _innmeldingOpprettelseLogic;

    public KnyttInnmeldingTilPersonController(IInnmeldingOpprettelseLogic innmeldingOpprettelseLogic)
    {
        _innmeldingOpprettelseLogic = innmeldingOpprettelseLogic;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]  
    public async Task<IActionResult> SendInn(KnyttInnmeldingTilPersonViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("KnyttInnmeldingTilPerson", model);
        }

        try
        {
            var gjesteinnmelder = new GjesteinnmelderModel
            {
                Epost = model.Epost
            };

            var innmelding = new InnmeldingModel
            {
                Tittel = model.Tittel,
                Beskrivelse = model.Beskrivelse,
                Prioritet = model.ErNodEtatKritisk ? "høy" : "ikke_vurdert",
                Status = "ny"
            };

            var geometri = new GeometriModel
            {
                GeometriGeoJson = model.GeometriGeoJson
            };

            var resultat = await _innmeldingOpprettelseLogic.ValiderOgLagreKompletKartfeil(
                innmelding,
                geometri,
                gjesteinnmelder);

            if (resultat)
            {
                return RedirectToAction("Kvittering", "Home");
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Det oppstod en feil ved innsending. Vennligst prøv igjen.");
        }

        return View("KnyttInnmeldingTilPerson", model);
    }
}
