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
    private readonly IEnumLogic _enumLogic;  // Legg til denne

    public KartvisningEnInnmeldingSaksBController(
        IGeometriRepository geometriRepository,
        IDataSammenstillingSaksBRepository dataSammenstillingsRepo,
        IEnumLogic enumLogic)  // Legg til denne
    {
        _geometriRepository = geometriRepository;
        _dataSammenstillingsRepo = dataSammenstillingsRepo;
        _enumLogic = enumLogic;
    }

    [HttpGet]
    public async Task<IActionResult> KartvisningEnInnmeldingSaksB(int? innmeldingId, string innmeldingIds)
    {
        var alleSaker = new List<(InnmeldingModel, PersonModel, InnmelderModel, SaksbehandlerModel, Geometri)>();

        if (innmeldingId.HasValue)
        {
            var (innmelding, person, innmelder, saksbehandler) =
                await _dataSammenstillingsRepo.GetInnmeldingMedDetaljerAsync(innmeldingId.Value);
            var geometriData = await _geometriRepository.GetGeometriByInnmeldingIdAsync(innmeldingId.Value);

            if (innmelding != null)
            {
                // Formater enum verdier før de legges til
                innmelding.Status = _enumLogic.ConvertToDisplayFormat(innmelding.Status);
                innmelding.Prioritet = _enumLogic.ConvertToDisplayFormat(innmelding.Prioritet);
                innmelding.KartType = _enumLogic.ConvertToDisplayFormat(innmelding.KartType);
                if (innmelder != null)
                {
                    innmelder.InnmelderType = _enumLogic.ConvertToDisplayFormat(innmelder.InnmelderType);
                }

                alleSaker.Add((innmelding, person, innmelder, saksbehandler, geometriData));
            }
        }
        else if (!string.IsNullOrEmpty(innmeldingIds))
        {
            var idListe = innmeldingIds.Split(',').Select(int.Parse);

            foreach (var id in idListe)
            {
                var (innmelding, person, innmelder, saksbehandler) =
                    await _dataSammenstillingsRepo.GetInnmeldingMedDetaljerAsync(id);
                var geometriData = await _geometriRepository.GetGeometriByInnmeldingIdAsync(id);

                if (innmelding != null)
                {
                    // Formater enum verdier før de legges til
                    innmelding.Status = _enumLogic.ConvertToDisplayFormat(innmelding.Status);
                    innmelding.Prioritet = _enumLogic.ConvertToDisplayFormat(innmelding.Prioritet);
                    innmelding.KartType = _enumLogic.ConvertToDisplayFormat(innmelding.KartType);
                    if (innmelder != null)
                    {
                        innmelder.InnmelderType = _enumLogic.ConvertToDisplayFormat(innmelder.InnmelderType);
                    }

                    alleSaker.Add((innmelding, person, innmelder, saksbehandler, geometriData));
                }
            }
        }

        if (!alleSaker.Any())
        {
            return NotFound("Ingen innmeldinger funnet");
        }

        var viewModel = new KartvisningEnInnmeldingSaksBViewModel
        {
            AlleInnmeldinger = alleSaker
        };

        return View(viewModel);
    }
}