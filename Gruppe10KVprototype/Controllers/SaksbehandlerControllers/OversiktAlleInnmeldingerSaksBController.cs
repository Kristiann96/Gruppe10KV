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
        public async Task<IActionResult> OversiktAlleInnmeldingerSaksB(int pageNumber = 1, int pageSize = 10)
        {
            IEnumerable<InnmeldingModel> innmeldinger = await _innmeldingRepository.GetOversiktAlleInnmeldingerSaksBAsync(pageNumber, pageSize);
            var totalItems = await _innmeldingRepository.GetTotalInnmeldingerTellerSaksBAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var viewModel = new OversiktAlleInnmeldingerSaksBViewModel
            {
                Innmeldinger = innmeldinger,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
            
            return View("OversiktAlleInnmeldingerSaksB", viewModel);
        }
    }
}
