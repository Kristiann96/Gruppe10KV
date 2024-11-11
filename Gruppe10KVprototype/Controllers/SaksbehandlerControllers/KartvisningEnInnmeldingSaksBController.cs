using Interface;
using Interfaces;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models;
using ViewModels;


public class KartvisningEnInnmeldingSaksBController : Controller
{
    private readonly IGeometriRepository _geometriRepository;
    private readonly IDataSammenstillingSaksBRepository _dataSammenstillingsRepo;
    private readonly IEnumLogic _enumLogic;
    private readonly IVurderingRepository _vurderingRepository;

    public KartvisningEnInnmeldingSaksBController(
        IGeometriRepository geometriRepository,
        IDataSammenstillingSaksBRepository dataSammenstillingsRepo,
        IEnumLogic enumLogic,
        IVurderingRepository vurderingRepository)
    {
        _geometriRepository = geometriRepository;
        _dataSammenstillingsRepo = dataSammenstillingsRepo;
        _enumLogic = enumLogic;
        _vurderingRepository = vurderingRepository;
    }

    [HttpGet]
    public async Task<IActionResult> KartvisningEnInnmeldingSaksB(int? innmeldingId, string innmeldingIds)
    {
        var viewModel = new KartvisningEnInnmeldingSaksBViewModel();

        async Task<InnmeldingMedDetaljerViewModel> HentInnmeldingMedVurderinger(int id)
        {
            var (innmelding, person, innmelder, saksbehandler) =
                await _dataSammenstillingsRepo.GetInnmeldingMedDetaljerAsync(id);
            var geometriData = await _geometriRepository.GetGeometriByInnmeldingIdAsync(id);

            if (innmelding == null) return null;

            // Formater enum verdier
            innmelding.Status = _enumLogic.ConvertToDisplayFormat(innmelding.Status);
            innmelding.Prioritet = _enumLogic.ConvertToDisplayFormat(innmelding.Prioritet);
            innmelding.KartType = _enumLogic.ConvertToDisplayFormat(innmelding.KartType);
            if (innmelder != null)
            {
                innmelder.InnmelderType = _enumLogic.ConvertToDisplayFormat(innmelder.InnmelderType);
            }

            // Hent vurderinger for denne innmeldingen
            var (antallBekreftelser, antallAvkreftelser) =
                await _vurderingRepository.HentAntallVurderingerAsync(id);
            var kommentarer = await _vurderingRepository.HentKommentarerForInnmeldingAsync(id);

            return new InnmeldingMedDetaljerViewModel
            {
                Innmelding = innmelding,
                Person = person,
                Innmelder = innmelder,
                Saksbehandler = saksbehandler,
                Geometri = geometriData,
                AntallBekreftelser = antallBekreftelser,
                AntallAvkreftelser = antallAvkreftelser,
                Kommentarer = kommentarer
            };
        }

        if (innmeldingId.HasValue)
        {
            var innmeldingDetaljer = await HentInnmeldingMedVurderinger(innmeldingId.Value);
            if (innmeldingDetaljer != null)
            {
                viewModel.AlleInnmeldinger.Add(innmeldingDetaljer);
            }
        }
        else if (!string.IsNullOrEmpty(innmeldingIds))
        {
            var idListe = innmeldingIds.Split(',').Select(int.Parse);
            foreach (var id in idListe)
            {
                var innmeldingDetaljer = await HentInnmeldingMedVurderinger(id);
                if (innmeldingDetaljer != null)
                {
                    viewModel.AlleInnmeldinger.Add(innmeldingDetaljer);
                }
            }
        }

        if (!viewModel.AlleInnmeldinger.Any())
        {
            return NotFound("Ingen innmeldinger funnet");
        }

        return View(viewModel);
    }
}