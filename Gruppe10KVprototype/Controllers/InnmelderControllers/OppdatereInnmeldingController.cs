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
                return Json(new { success = true });  // Returner JSON istedenfor redirect
            }
            catch (ValidationException ex)
            {
                return Json(new { success = false, message = ex.Message });
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
