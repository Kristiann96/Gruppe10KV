using Microsoft.AspNetCore.Mvc;
using ViewModels;

[AutoValidateAntiforgeryToken]
public class KartfeilSkjemaBekreftelseController : Controller
{
    public IActionResult KartfeilSkjemaBekreftelse(KartfeilSkjemaViewModel model)
    {
        if (string.IsNullOrEmpty(model.GeometriGeoJson))
        {
            return RedirectToAction("KartfeilMarkering", "KartfeilMarkering");
        }
        return View(model);
    }
}