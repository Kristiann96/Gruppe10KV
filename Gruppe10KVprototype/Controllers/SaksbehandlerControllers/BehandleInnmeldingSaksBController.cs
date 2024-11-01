using Interface;
using Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ViewModels;
using Models.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class BehandleInnmeldingSaksBController : Controller
    {
        private readonly IInnmeldingRepository _innmeldingRepository;
        private readonly IGeometriRepository _geometriRepository;
        private readonly IInnmeldingEnumLogic _innmeldingEnumLogic;

        public BehandleInnmeldingSaksBController(IInnmeldingRepository innmeldingRepository,
            IGeometriRepository geometriRepository,
            IInnmeldingEnumLogic innmeldingEnumLogic)
        {
            _innmeldingRepository = innmeldingRepository;
            _geometriRepository = geometriRepository;
            _innmeldingEnumLogic = innmeldingEnumLogic;
        }

        [HttpGet]
        public async Task<IActionResult> BehandleInnmeldingSaksB()
        {
            int innmeldingId = 10; // Fetch details for InnmeldingID 10
            var innmeldingModel = await _innmeldingRepository.GetInnmeldingByIdAsync(innmeldingId);
            var geometri = await _geometriRepository.GetGeometriByInnmeldingIdAsync(innmeldingId);
            var statusOptions = await _innmeldingEnumLogic.GetFormattedStatusEnumValuesAsync();

            if (innmeldingModel == null)
            {
                return NotFound("Innmelding details not found.");
            }

            var viewModel = new BehandleInnmeldingSaksBViewModel
            {
                InnmeldingModel = innmeldingModel,
                Geometri = geometri,
                StatusOptions = statusOptions.Select(so => new SelectListItem { Value = so, Text = so }).ToList()
            };

            return View(viewModel);
        }
    }
}