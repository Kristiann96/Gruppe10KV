using AuthInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesInterfaces;

[Authorize(Roles = UserRoles.Saksbehandler)]
[AutoValidateAntiforgeryToken]
public class KartvisningEnEllerFlereInnmeldingSaksBController : Controller
{
    private readonly IKartvisningEnEllerFlereInnmeldingSaksBService _kartvisningService;

    public KartvisningEnEllerFlereInnmeldingSaksBController(
        IKartvisningEnEllerFlereInnmeldingSaksBService kartvisningService)
    {
        _kartvisningService = kartvisningService;
    }

    [HttpGet]
    public async Task<IActionResult> KartvisningEnEllerFlereInnmeldingSaksB(int? innmeldingId, string innmeldingIds)
    {
        if (innmeldingId.HasValue)
        {
            var viewModel = await _kartvisningService.HentKartvisningForEnkeltInnmeldingAsync(innmeldingId.Value);
            if (!viewModel.AlleInnmeldinger.Any())
            {
                return NotFound("Ingen innmelding funnet");
            }
            return View(viewModel);
        }

        if (!string.IsNullOrEmpty(innmeldingIds))
        {
            var idListe = innmeldingIds.Split(',').Select(int.Parse);
            var viewModel = await _kartvisningService.HentKartvisningForFlereInnmeldingerAsync(idListe);
            if (!viewModel.AlleInnmeldinger.Any())
            {
                return NotFound("Ingen innmeldinger funnet");
            }
            return View(viewModel);
        }

        return NotFound("Ingen innmeldinger spesifisert");
    }
}