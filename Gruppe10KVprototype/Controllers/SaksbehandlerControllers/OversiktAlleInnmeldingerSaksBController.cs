using Interface;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class OversiktAlleInnmeldingerSaksBController : Controller
    {
        private readonly IInnmeldingRepository _innmeldingRepository;
        
        public OversiktAlleInnmeldingerSaksBController(IInnmeldingRepository innmeldingRepository)
        {
            _innmeldingRepository = innmeldingRepository;
        }
        public async Task<IActionResult> OversiktAlleInnmeldingerSaksB()
        {
            IEnumerable<InnmeldingModel> innmeldinger = await _innmeldingRepository.GetOversiktAlleInnmeldingerSaksBAsync();
            
            return View("OversiktAlleInnmeldingerSaksB", innmeldinger);
        }
    }
}
