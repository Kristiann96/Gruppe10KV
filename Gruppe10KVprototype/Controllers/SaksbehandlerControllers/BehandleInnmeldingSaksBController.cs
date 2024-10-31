using Interface;
using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers
{
    public class BehandleInnmeldingSaksBController : Controller
    {
        private readonly IInnmeldingRepository _innmeldingRepository;

        public BehandleInnmeldingSaksBController(IInnmeldingRepository innmeldingRepository)
        {
            _innmeldingRepository = innmeldingRepository;
        }
        
        /*public IActionResult BehandleInnmeldingSaksB()
        {
            return View();
        }*/
        
        public async Task<IActionResult> BehandleInnmeldingSaksB(int id)
        {
            var innmelding = await _innmeldingRepository.GetInnmeldingByIdAsync(id);
            if (innmelding == null)
            {
                return NotFound();
            }
            return View("BehandleInnmeldingSaksB", innmelding);
        }
    }
}