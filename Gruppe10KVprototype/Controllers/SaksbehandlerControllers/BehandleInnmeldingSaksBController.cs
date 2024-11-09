
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
using LogicInterfaces;
using Models.Entities;


namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers;

public class BehandleInnmeldingSaksBController : Controller
{
    private readonly IGeometriRepository _geometriRepository;
    private readonly IEnumLogic _enumLogic;
    private readonly IDataSammenstillingSaksBRepository _dataSammenstillingSaksBRepository;
    private readonly IInnmeldingRepository _innmeldingRepository; // Ny: Trenger for oppdatering
    private readonly ISaksbehandlerRepository _saksbehandlerRepository;

    public BehandleInnmeldingSaksBController(
        IGeometriRepository geometriRepository,
        IEnumLogic enumLogic,
        IDataSammenstillingSaksBRepository dataSammenstillingSaksBRepository,
        IInnmeldingRepository innmeldingRepository,
        ISaksbehandlerRepository saksbehandlerRepository)
    {
        _geometriRepository = geometriRepository;
        _enumLogic = enumLogic;
        _dataSammenstillingSaksBRepository = dataSammenstillingSaksBRepository;
        _innmeldingRepository = innmeldingRepository;
        _saksbehandlerRepository = saksbehandlerRepository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BehandleInnmeldingSaksB(int id)
    {
        var (innmelding, person, innmelder, saksbehandler) =
            await _dataSammenstillingSaksBRepository.GetInnmeldingMedDetaljerAsync(id);

        if (innmelding == null)
        {
            return RedirectToAction("OversiktAlleInnmeldingerSaksB", "OversiktAlleInnmeldingerSaksB");
        }

        var geometri = await _geometriRepository.GetGeometriByInnmeldingIdAsync(id);

        // Hent alle enum verdier
        var statusOptions = await _enumLogic.GetFormattedStatusEnumValuesAsync();
        var prioritetOptions = await _enumLogic.GetFormattedPrioritetEnumValuesAsync();
        var kartTypeOptions = await _enumLogic.GetFormattedKartTypeEnumValuesAsync();

        var viewModel = new BehandleInnmeldingSaksBViewModel
        {
            InnmeldingModel = innmelding,
            PersonModel = person,
            InnmelderModel = innmelder,
            SaksbehandlerModel = saksbehandler,
            Geometri = geometri,
            StatusOptions = statusOptions.Select(so => new SelectListItem { Value = so, Text = so }).ToList(),
            PrioritetOptions = prioritetOptions.Select(po => new SelectListItem { Value = po, Text = po }).ToList(),
            KartTypeOptions = kartTypeOptions.Select(ko => new SelectListItem { Value = ko, Text = ko }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> OppdateringAvInnmeldingSaksB(int id,
        BehandleInnmeldingSaksBViewModel viewModel)
    {
        try
        {
            var model = viewModel.InnmeldingModel;

            // Konverter verdiene tilbake til database format
            model.Status = _enumLogic.ConvertToDbFormat(model.Status);
            model.Prioritet = _enumLogic.ConvertToDbFormat(model.Prioritet);
            model.KartType = _enumLogic.ConvertToDbFormat(model.KartType);

            // Valider verdiene //burde refactoreres bort fra controller??!
            var isStatusValid = await _enumLogic.ValidateStatusValueAsync(model.Status);
            var isPrioritetValid = await _enumLogic.ValidatePrioritetValueAsync(model.Prioritet);
            var isKartTypeValid = await _enumLogic.ValidateKartTypeValueAsync(model.KartType);

            if (!isStatusValid || !isPrioritetValid || !isKartTypeValid)
            {
                TempData["ErrorMessage"] = "En eller flere verdier er ugyldige";
                return RedirectToAction(nameof(BehandleInnmeldingSaksB), new { id });
            }

            var result = await _innmeldingRepository.OppdatereEnumSaksBAsync(id, model);

            if (result)
            {
                TempData["SuccessMessage"] = "Lagret"; // Lagt til denne
            }
            else
            {
                TempData["ErrorMessage"] = "Lagring feilet";
            }

            return RedirectToAction(nameof(BehandleInnmeldingSaksB), new { id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Lagring feilet";
            return RedirectToAction(nameof(BehandleInnmeldingSaksB), new { id });
        }
    }


    public async Task<IActionResult> AngiSaksbahandler(string saksbehandlerId)
        {
            var (Navn, Id) = await _saksbehandlerRepository.HentAlleSaksbehandlereNavnId();
            Navn = Navn.Select(so => new SelectListItem { Value = Id, Text = so }).ToList(),

        }
    
}

