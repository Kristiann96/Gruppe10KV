using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ViewModels;
using ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using AuthInterface;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{

    [Authorize(Roles = UserRoles.Innmelder)]
    [AutoValidateAntiforgeryToken]
    public class OppdatereInnmeldingController : Controller
    {
        private readonly IOppdatereInnmeldingService _innmeldingService;

        public OppdatereInnmeldingController(IOppdatereInnmeldingService innmeldingService)
        {
            _innmeldingService = innmeldingService;
        }

        [HttpGet]
        public async Task<IActionResult> OppdatereInnmelding(int innmeldingId)
        {
            try
            {
                var viewModel = await _innmeldingService.HentInnmeldingForOppdateringAsync(innmeldingId);
                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                // Ved ikke-eksisterende innmelding, redirect til liste med feilmelding
                TempData["ErrorMessage"] = "Innmelding ikke funnet";
                return RedirectToAction("MineInnmeldinger", "MineInnmeldinger");
            }
            catch (Exception)
            {
                // Ved uventet feil, redirect til liste med generisk feilmelding
                TempData["ErrorMessage"] = "En feil oppstod ved henting av innmelding";
                return RedirectToAction("MineInnmeldinger", "MineInnmeldinger");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OppdatereInnmeldingDetaljer([FromForm] OppdatereInnmeldingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Validering feilet. Sjekk at alle påkrevde felt er fylt ut."
                });
            }

            try
            {
                await _innmeldingService.OppdatereInnmeldingAsync(model);
                return Json(new { success = true });
            }
            catch (ValidationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return Json(new { success = false, message = "Innmelding ikke funnet" });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "En uventet feil oppstod ved oppdatering av innmeldingen"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OppdatereGeometri([FromBody] OppdatereInnmeldingViewModel model)
        {
            try
            {
                await _innmeldingService.OppdatereGeometriAsync(model.InnmeldingId, model.GeometriGeoJson);
                return Json(new { success = true });
            }
            catch (ValidationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return Json(new { success = false, message = "Innmelding ikke funnet" });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "En uventet feil oppstod ved oppdatering av geometrien"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SlettInnmelding(int innmeldingId)
        {
            try
            {
                var result = await _innmeldingService.SlettInnmeldingAsync(innmeldingId);
                return Json(new { success = result });
            }
            catch (KeyNotFoundException)
            {
                return Json(new
                {
                    success = false,
                    message = "Innmelding ikke funnet"
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "Kunne ikke slette innmeldingen, om feilen vedvarer vennligst ta kontakt"
                });
            }
        }
    }
}
