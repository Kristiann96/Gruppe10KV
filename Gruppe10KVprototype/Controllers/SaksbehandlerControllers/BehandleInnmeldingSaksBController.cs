
using Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ViewModels;
using Models.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Interface;
using Interfaces;



namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class BehandleInnmeldingSaksBController : Controller
    {

        private readonly IGeometriRepository _geometriRepository;
        private readonly IInnmeldingEnumLogic _innmeldingEnumLogic;
        private readonly IDataSammenstillingSaksBRepository _dataSammenstillingSaksBRepository;
        private readonly IInnmeldingRepository _innmeldingRepository;

        public BehandleInnmeldingSaksBController(IInnmeldingRepository innmeldingRepository,

            IGeometriRepository geometriRepository,
            IInnmeldingEnumLogic innmeldingEnumLogic,
            IDataSammenstillingSaksBRepository dataSammenstillingSaksBRepository)
        {
            _innmeldingRepository = innmeldingRepository;
            _geometriRepository = geometriRepository;
            _innmeldingEnumLogic = innmeldingEnumLogic;
            _dataSammenstillingSaksBRepository = dataSammenstillingSaksBRepository;

        }

        [HttpGet]
        public async Task<IActionResult> BehandleInnmeldingSaksB(int? id)
        {
            // Sjekk at vi har fått en gyldig id
            if (!id.HasValue)
            {
                return BadRequest("Ingen innmelding ID spesifisert.");
            }

            try
            {
                // Hent sammenstilt data
                var (innmelding, person, innmelder, saksbehandler) =
                    await _dataSammenstillingSaksBRepository.GetInnmeldingMedDetaljerAsync(id.Value);

                if (innmelding == null)
                {
                    return NotFound($"Fant ikke innmelding med ID: {id}");
                }

                // Hent tilleggsdata
                var geometri = await _geometriRepository.GetGeometriByInnmeldingIdAsync(id.Value);
                var statusOptions = await _innmeldingEnumLogic.GetFormattedStatusEnumValuesAsync();

                // Bygg viewmodel
                var viewModel = new BehandleInnmeldingSaksBViewModel
                {
                    InnmeldingModel = innmelding,
                    PersonModel = person,
                    InnmelderModel = innmelder,
                    SaksbehandlerModel = saksbehandler,
                    Geometri = geometri,
                    StatusOptions = statusOptions.Select(so => new SelectListItem { Value = so, Text = so }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Logger feilen hvis du har logging implementert
                // _logger.LogError(ex, $"Feil ved henting av innmelding {id}");

                // Redirect til en feilside eller tilbake til listen
                TempData["ErrorMessage"] = "Det oppstod en feil ved lasting av innmeldingen.";
                return RedirectToAction("Index"); // eller hvor du vil redirecte ved feil
            }
        }
    }
}