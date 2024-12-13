using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Registrering;
using Interface;
using AuthInterface;
using System.Transactions;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    [AutoValidateAntiforgeryToken]
    public class RegistrerDegController : Controller
    {
        private readonly ITransaksjonsRepository _transaksjonsRepository;
        private readonly IAuthService _authService;
        private readonly ILogger<RegistrerDegController> _logger;

        public RegistrerDegController(
            ITransaksjonsRepository transaksjonsRepository,
            IAuthService authService,
            ILogger<RegistrerDegController> logger)
        {
            _transaksjonsRepository = transaksjonsRepository;
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegistrerDeg()
        {
            return View(new KomplettRegistreringViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrerInnmelder(KomplettRegistreringViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = string.Join(" ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage))
                });
            }

            try
            {
                if (await _authService.DoesEmailExistAsync(model.Email))
                {
                    return Json(new
                    {
                        success = false,
                        message = "E-post er registrert, vennligst velg en annen"
                    });
                }

                var (success, personId, errorMessage) = await _transaksjonsRepository.OpprettPersonOgInnmelder(
                    model.Fornavn,
                    model.Etternavn,
                    model.Telefonnummer,
                    model.Email);

                if (!success)
                {
                    return Json(new
                    {
                        success = false,
                        message = errorMessage ?? "Registrering feilet"
                    });
                }

                var identityResult = await _authService.RegisterInnmelderAsync(
                    model.Email,
                    model.Password,
                    personId);

                if (!identityResult.success)
                {
                    // TODO: Her burde vi egentlig rulle tilbake person/innmelder opprettelsen
                    return Json(new
                    {
                        success = false,
                        message = string.Join(" ", identityResult.errors)
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "Registrering vellykket! Du vil nå bli videresendt til innlogging.",
                    redirectUrl = Url.Action("LandingsSide", "LandingsSide")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feil under registrering av bruker");
                return Json(new
                {
                    success = false,
                    message = "Det oppstod en teknisk feil under registrering. Vennligst prøv igjen senere."
                });
            }
        }
    }
}