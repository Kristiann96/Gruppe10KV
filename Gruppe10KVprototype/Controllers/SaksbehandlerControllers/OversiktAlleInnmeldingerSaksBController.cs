using Interface;
using Interfaces;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using ViewModels;

public class OversiktAlleInnmeldingerSaksBController : Controller
{
    private readonly IDataSammenstillingSaksBRepository _dataSammenstillingSaksBRepository;
    private readonly IGeometriRepository _geometriRepository;
    private readonly IKommuneAPILogic _kommuneAPILogic;
    private readonly IEnumLogic _enumLogic;

    public OversiktAlleInnmeldingerSaksBController(
        IDataSammenstillingSaksBRepository dataSammenstillingSaksBRepository,
        IGeometriRepository geometriRepository,
        IKommuneAPILogic kommuneAPILogic,
        IEnumLogic enumLogic)
    {
        _dataSammenstillingSaksBRepository = dataSammenstillingSaksBRepository;
        _geometriRepository = geometriRepository;
        _kommuneAPILogic = kommuneAPILogic;
        _enumLogic = enumLogic;
    }

    public async Task<IActionResult> OversiktAlleInnmeldingerSaksB(int pageNumber = 1, int pageSize = 10, string searchTerm = "")
    {
        var result =
            await _dataSammenstillingSaksBRepository.GetOversiktAlleInnmeldingerSaksBAsync(pageNumber, pageSize, searchTerm);
        var innmeldinger = result.Data;
        var totalPages = result.TotalPages;

        if (innmeldinger == null || !innmeldinger.Any())
        {
            return View("OversiktAlleInnmeldingerSaksB", new OversiktAlleInnmeldingerSaksBViewModel());
        }

        var kommuneData = new List<string>();
        foreach (var innmelding in innmeldinger)
        {
            var kommune = await GetKommuneFraInnmelding(innmelding.Item1?.InnmeldingId ?? 0);
            kommuneData.Add(kommune);
        }

        var viewModel = new OversiktAlleInnmeldingerSaksBViewModel
        {
            Innmeldinger = innmeldinger.Select(i =>
            {
                var innmelding = i.Item1 ?? new InnmeldingModel();
                innmelding.Status = _enumLogic.ConvertToDisplayFormat(innmelding.Status);
                innmelding.Prioritet = _enumLogic.ConvertToDisplayFormat(innmelding.Prioritet);
                return innmelding;
            }),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages,
            SearchTerm = searchTerm,
            InnmelderNavn = innmeldinger.Select(i =>
                string.IsNullOrEmpty(i.Item2?.Fornavn) && string.IsNullOrEmpty(i.Item2?.Etternavn)
                    ? "Gjest"
                    : $"{i.Item2?.Fornavn} {i.Item2?.Etternavn}"),
            InnmelderEpost = innmeldinger.Select(i =>
                !string.IsNullOrEmpty(i.Item5?.Epost) ? i.Item5.Epost : "N/A"),
            GjestEpost = innmeldinger.Select(i =>
                !string.IsNullOrEmpty(i.Item4?.Epost) ? i.Item4.Epost : "N/A"),
            KommuneData = kommuneData
        };

        return View("OversiktAlleInnmeldingerSaksB", viewModel);
    }

    private async Task<string> GetKommuneFraInnmelding(int innmeldingId)
    {
        try
        {
            var geometri = await _geometriRepository.GetGeometriByInnmeldingIdAsync(innmeldingId);
            if (geometri == null)
                return "Ikke tilgjengelig";

            return await _kommuneAPILogic.GetKommuneStringFromGeometri(geometri);
        }
        catch (Exception)
        {
            return "Ikke tilgjengelig";
        }
    }
}