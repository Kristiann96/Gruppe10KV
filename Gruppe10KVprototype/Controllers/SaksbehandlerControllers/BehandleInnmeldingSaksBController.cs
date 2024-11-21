
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
    private readonly IInnmeldingRepository _innmeldingRepository;
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
        // 1. Hent hoveddata først
        var (innmelding, person, innmelder, saksbehandler) =
            await _dataSammenstillingSaksBRepository.GetInnmeldingMedDetaljerAsync(id);

        if (innmelding == null)
        {
            return RedirectToAction("OversiktAlleInnmeldingerSaksB", "OversiktAlleInnmeldingerSaksB");
        }

        // 2. Hent geometri
        var geometri = await _geometriRepository.GetGeometriByInnmeldingIdAsync(id);

        // 3. Hent enum-verdier
        var statusOptions = await _enumLogic.GetFormattedStatusEnumValuesAsync();
        var prioritetOptions = await _enumLogic.GetFormattedPrioritetEnumValuesAsync();
        var kartTypeOptions = await _enumLogic.GetFormattedKartTypeEnumValuesAsync();
        var innmelderOptions = await _enumLogic.GetFormattedInnmelderTypeEnumValuesAsync();

        // 4. Hent saksbehandlere for dropdown
        var saksbehandlereMedPerson = await _saksbehandlerRepository.HentAlleSaksbehandlereMedPersonAsync();

        var viewModel = new BehandleInnmeldingSaksBViewModel
        {
            InnmeldingModel = innmelding,
            PersonModel = person,
            InnmelderModel = innmelder,
            SaksbehandlerModel = saksbehandler,
            Geometri = geometri,
            StatusOptions = statusOptions.Select(so => new SelectListItem { Value = so, Text = so }).ToList(),
            PrioritetOptions = prioritetOptions.Select(po => new SelectListItem { Value = po, Text = po }).ToList(),
            KartTypeOptions = kartTypeOptions.Select(ko => new SelectListItem { Value = ko, Text = ko }).ToList(),
            InnmelderTypeOptions = innmelderOptions.Select(i => new SelectListItem
            { Value = i, Text = i, Selected = innmelder?.InnmelderType == i }).ToList(),
            ValgtSaksbehandlerId = saksbehandler?.SaksbehandlerId,
            SaksbehandlereMedPerson = saksbehandlereMedPerson
        };

        return View(viewModel);
    }

    [HttpGet("HentSaksbehandlere")]
    public async Task<IActionResult> HentSaksbehandlereForDropdown()
    {
        var saksbehandlereMedPerson = await _saksbehandlerRepository.HentAlleSaksbehandlereMedPersonAsync();

        return Json(saksbehandlereMedPerson.Select(s => new
        {
            Value = s.Item1.SaksbehandlerId.ToString(),
            Text = $"{s.Item2.Fornavn} {s.Item2.Etternavn}"
        }));
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


    [HttpPost]
    public async Task<IActionResult> LagreSaksbehandler(int innmeldingId, int? saksbehandlerId)
    {
        try
        {
            var result = await _innmeldingRepository.OppdaterSaksbehandler(innmeldingId, saksbehandlerId);

            if (result)
            {
                TempData["SuccessMessage"] = "Saksbehandler er oppdatert";
            }
            else
            {
                TempData["ErrorMessage"] = "Kunne ikke finne innmeldingen";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Kunne ikke oppdatere saksbehandler";
        }

        return RedirectToAction(nameof(BehandleInnmeldingSaksB), new { id = innmeldingId });
    }

    [HttpPost]
    public async Task<IActionResult> OppdaterInnmelderType(string innmelderType, int innmelderId, int innmeldingId)
    {
        try
        {
            // Konverter til database format
            var dbInnmelderType = _enumLogic.ConvertToDbFormat(innmelderType);

            // Valider
            var isValid = await _enumLogic.ValidateInnmelderTypeValueAsync(dbInnmelderType);
            if (!isValid)
            {
                TempData["ErrorMessage"] = "Ugyldig innmelder type";
                return await BehandleInnmeldingSaksB(innmeldingId);
            }

            // Oppdater
            var model = new InnmeldingModel { InnmelderType = dbInnmelderType };
            var result = await _innmeldingRepository.OppdaterInnmelderType(innmelderId, model);

            if (result)
            {
                TempData["SuccessMessage"] = "Innmeldertype er oppdatert";
            }
            else
            {
                TempData["ErrorMessage"] = "Kunne ikke finne innmelderen";
            }

            // Returner til samme view med oppdatert data
            return RedirectToAction(nameof(BehandleInnmeldingSaksB), new { id = innmeldingId });
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Kunne ikke oppdatere innmeldertype";
            return RedirectToAction(nameof(BehandleInnmeldingSaksB), new { id = innmeldingId });
        }
    }
}



