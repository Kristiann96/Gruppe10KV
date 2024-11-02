using Microsoft.AspNetCore.Mvc;

public class KartfeilSkjemaController : Controller
{
    public IActionResult KartfeilSkjema(string geoJson)
    {
        if (string.IsNullOrEmpty(geoJson))
        {
            return RedirectToAction("KartfeilMarkering", "KartfeilMarkering");
        }

        var viewModel = new KartfeilSkjemaViewModel
        {
            GeometriGeoJson = geoJson,
            Tittel = "",
            Beskrivelse = "",
            ErNodEtatKritisk = false // For prioritet enum
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult GaaTilBekreftelse(KartfeilSkjemaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("KartfeilSkjema", model);
        }

        // Setter prioritet basert på checkbox
        model.Prioritet = model.ErNodEtatKritisk ? "høy" : "ikke_vurdert";

        // Sender videre til bekreftelse
        return RedirectToAction("KartfeilSkjemaBekreftelse", "KartfeilSkjemaBekreftelse", model);
    }
}