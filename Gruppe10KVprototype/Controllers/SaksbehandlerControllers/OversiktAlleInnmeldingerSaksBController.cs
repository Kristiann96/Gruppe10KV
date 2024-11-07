using Interface;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class OversiktAlleInnmeldingerSaksBController : Controller
    {
        private readonly IDataSammenstillingSaksBRepository _dataSammenstillingSaksBRepository;

        public OversiktAlleInnmeldingerSaksBController(
            IDataSammenstillingSaksBRepository dataSammenstillingSaksBRepository)
        {
            _dataSammenstillingSaksBRepository = dataSammenstillingSaksBRepository;
        }

        public async Task<IActionResult> OversiktAlleInnmeldingerSaksB(int pageNumber = 1, int pageSize = 10,
            string searchTerm = "")
        {
            var result =
                await _dataSammenstillingSaksBRepository.GetOversiktAlleInnmeldingerSaksBAsync(pageNumber, pageSize,
                    searchTerm);
            var innmeldinger = result.Data;
            var totalPages = result.TotalPages;

            if (innmeldinger == null || !innmeldinger.Any())
            {
                return View("OversiktAlleInnmeldingerSaksB", new OversiktAlleInnmeldingerSaksBViewModel());
            }

            var viewModel = new OversiktAlleInnmeldingerSaksBViewModel
            {
                Innmeldinger = innmeldinger.Select(i => i.Item1 ?? new InnmeldingModel()),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                SearchTerm = searchTerm,
                InnmelderNavn = innmeldinger.Select(i =>
                    string.IsNullOrEmpty(i.Item2?.Fornavn) && string.IsNullOrEmpty(i.Item2?.Etternavn)
                        ? "Gjest"
                        : $"{i.Item2?.Fornavn} {i.Item2?.Etternavn}"),
                // Updated email handling
                InnmelderEpost = innmeldinger.Select(i =>
                    !string.IsNullOrEmpty(i.Item5?.Epost) ? i.Item5.Epost : "N/A"),
                GjestEpost = innmeldinger.Select(i =>
                    !string.IsNullOrEmpty(i.Item4?.Epost) ? i.Item4.Epost : "N/A")
            };

            return View("OversiktAlleInnmeldingerSaksB", viewModel);
        }
    }
}
