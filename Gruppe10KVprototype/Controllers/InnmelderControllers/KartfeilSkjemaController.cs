using Microsoft.AspNetCore.Mvc;


[AutoValidateAntiforgeryToken]
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
            ErNodEtatKritisk = false 
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult GaaTilBekreftelse(KartfeilSkjemaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("KartfeilSkjema", model);
        }

        model.Prioritet = model.ErNodEtatKritisk ? "høy" : "ikke_vurdert";

        return View("KartfeilSkjema", model);
    }
}