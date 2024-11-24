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
using AuthInterface;
using Microsoft.AspNetCore.Authorization;


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
        try
        {
            // 1. Hent hoveddata først
            var (innmelding, person, innmelder, saksbehandler) =
                await _dataSammenstillingSaksBRepository.GetInnmeldingMedDetaljerAsync(id);

            if (innmelding == null)
            {
                TempData["ErrorMessage"] = "Innmelding ikke funnet";
                return RedirectToAction("OversiktAlleInnmeldingerSaksB", "OversiktAlleInnmeldingerSaksB");
            }

            // 2. Hent geometri
            var geometri = await _geometriRepository.GetGeometriByInnmeldingIdAsync(id);

            // 3. Hent enum-verdier med feilhåndtering
            var statusOptions = await TryGetEnumOptions(_enumLogic.GetFormattedStatusEnumValuesAsync);
            var prioritetOptions = await TryGetEnumOptions(_enumLogic.GetFormattedPrioritetEnumValuesAsync);
            var kartTypeOptions = await TryGetEnumOptions(_enumLogic.GetFormattedKartTypeEnumValuesAsync);
            var innmelderOptions = await TryGetEnumOptions(_enumLogic.GetFormattedInnmelderTypeEnumValuesAsync);

            // 4. Hent saksbehandlere med feilhåndtering
            var saksbehandlereMedPerson = await _saksbehandlerRepository.HentAlleSaksbehandlereMedPersonAsync()
                ?? new List<(SaksbehandlerModel, PersonModel)>();

            var viewModel = new BehandleInnmeldingSaksBViewModel
            {
                InnmeldingModel = innmelding,
                PersonModel = person,
                InnmelderModel = innmelder,
                SaksbehandlerModel = saksbehandler,
                Geometri = geometri,
                StatusOptions = statusOptions,
                PrioritetOptions = prioritetOptions,
                KartTypeOptions = kartTypeOptions,
                InnmelderTypeOptions = innmelderOptions,
                ValgtSaksbehandlerId = saksbehandler?.SaksbehandlerId,
                SaksbehandlereMedPerson = saksbehandlereMedPerson
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
           
            TempData["ErrorMessage"] = "Det oppstod en feil ved lasting av innmeldingen";
            return RedirectToAction("OversiktAlleInnmeldingerSaksB", "OversiktAlleInnmeldingerSaksB");
        }
    }

    private static async Task<List<SelectListItem>> TryGetEnumOptions(Func<Task<IEnumerable<string>>> getEnumValues)
    {
        try
        {
            var values = await getEnumValues();
            return values?.Select(v => new SelectListItem { Value = v, Text = v }).ToList()
                ?? new List<SelectListItem>();
        }
        catch
        {
            return new List<SelectListItem>();
        }
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



