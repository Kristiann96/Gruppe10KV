using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ViewModels;
using ServicesInterfaces;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{

  
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
                return NotFound("Innmelding ikke funnet");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OppdatereInnmeldingDetaljer(OppdatereInnmeldingViewModel model)
        {
            if (!ModelState.IsValid)
                return View("OppdatereInnmelding", model);

            try
            {
                await _innmeldingService.OppdatereInnmeldingAsync(model);
                return RedirectToAction("Success");
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("OppdatereInnmelding", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OppdatereGeometri(int innmeldingId, string geometriGeoJson)
        {
            try
            {
                await _innmeldingService.OppdatereGeometriAsync(innmeldingId, geometriGeoJson);
                return Json(new { success = true });
            }
            catch (ValidationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SlettInnmelding(int innmeldingId)
        {
            try
            {
                await _innmeldingService.SlettInnmeldingAsync(innmeldingId);
                return RedirectToAction("Index", "Home"); // eller hvor du vil redirecte etter sletting
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Innmelding ikke funnet");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Kunne ikke slette innmeldingen: " + ex.Message);
                // Redirect tilbake til view med feilmelding
                return RedirectToAction("OppdatereInnmelding", new { innmeldingId });
            }
        }
    }
}
