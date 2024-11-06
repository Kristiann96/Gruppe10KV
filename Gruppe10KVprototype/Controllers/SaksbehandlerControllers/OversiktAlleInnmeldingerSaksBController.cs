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
        
        public OversiktAlleInnmeldingerSaksBController(IDataSammenstillingSaksBRepository dataSammenstillingSaksBRepository)
        {
            _dataSammenstillingSaksBRepository = dataSammenstillingSaksBRepository;
        }
        public async Task<IActionResult> OversiktAlleInnmeldingerSaksB(int pageNumber = 1, int pageSize = 10, string searchTerm = "")
        {
            var result = await _dataSammenstillingSaksBRepository.GetOversiktAlleInnmeldingerSaksBAsync(pageNumber, pageSize, searchTerm);
            var innmeldinger = result.Data;
            var totalPages = result.TotalPages;
            
            if (innmeldinger == null || !innmeldinger.Any())
            {
                // Handle the case where the innmeldinger collection is null or empty
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
                InnmelderEpost = innmeldinger.Select(i => i.Item5?.Epost ?? "MÅ HA EPOST"),
                GjestEpost = innmeldinger.Select(i => i.Item4?.Epost ?? "MÅ HA EPOST")
            };
            
            return View("OversiktAlleInnmeldingerSaksB", viewModel);
        }
    }
}
