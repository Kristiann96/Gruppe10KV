
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

        public BehandleInnmeldingSaksBController(IInnmeldingRepository innmeldingRepository
            IGeometriRepository geometriRepository,
            IInnmeldingEnumLogic innmeldingEnumLogic,
            IDataSammenstillingSaksBRepository dataSammenstillingSaksBRepository)
        {
_innmeldingRepository = innmeldingRepository;
            _geometriRepository = geometriRepository;
            _innmeldingEnumLogic = innmeldingEnumLogic;
            _dataSammenstillingSaksBRepository = dataSammenstillingSaksBRepository;

        }

  /*public IActionResult BehandleInnmeldingSaksB()
        {
            return View();
        }*/
        
        public async Task<IActionResult> BehandleInnmeldingSaksB(int id)
        {
            var innmelding = await _innmeldingRepository.GetInnmeldingByIdAsync(id);
            if (innmelding == null)
            {
                return NotFound();
            }
            return View("BehandleInnmeldingSaksB", innmelding);

        [HttpGet]
        public async Task<IActionResult> BehandleInnmeldingSaksB()
        {

            int innmeldingId = 10;
            var (innmelding, person, innmelder, saksbehandler) =
                await _dataSammenstillingSaksBRepository.GetInnmeldingMedDetaljerAsync(innmeldingId);

            if (innmelding == null)

            {
                return NotFound("Innmelding details not found.");
            }


            var geometri = await _geometriRepository.GetGeometriByInnmeldingIdAsync(innmeldingId);
            var statusOptions = await _innmeldingEnumLogic.GetFormattedStatusEnumValuesAsync();

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
    }
}