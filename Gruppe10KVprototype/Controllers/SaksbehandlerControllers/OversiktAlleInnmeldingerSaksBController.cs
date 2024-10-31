using Interface;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class OversiktAlleInnmeldingerSaksBController : Controller
    {
        private readonly IInnmeldingRepository _innmeldingRepository;
        
        public OversiktAlleInnmeldingerSaksBController(IInnmeldingRepository innmeldingRepository)
        {
            _innmeldingRepository = innmeldingRepository;
        }
        public async Task<IActionResult> OversiktAlleInnmeldingerSaksB(int pageNumber = 1, int pageSize = 10, string searchTerm = "")
        {
            IEnumerable<InnmeldingModel> innmeldinger = await _innmeldingRepository.GetOversiktAlleInnmeldingerSaksBAsync(pageNumber, pageSize, searchTerm);
            var totalItems = await _innmeldingRepository.GetTotalInnmeldingerTellerSaksBAsync(searchTerm);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var viewModel = new OversiktAlleInnmeldingerSaksBViewModel
            {
                Innmeldinger = innmeldinger,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                SearchTerm = searchTerm
            };
            
            return View("OversiktAlleInnmeldingerSaksB", viewModel);
        }
    }
}
