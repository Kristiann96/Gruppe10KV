using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Models.Models;
using System.Threading.Tasks;
using Interface;
using System.Collections.Generic;
using ViewModels;
using System.Linq;
using Logic;
using Interfaces;
using DataAccess;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    [Route("OppdatereInnmelding/[controller]")]
    public class OppdatereInnmeldingController : Controller
    {
        private readonly IInnmeldingRepository _innmeldingRepository;
        private readonly IGeometriRepository _geometriRepository;
        //private readonly IInnmeldingRepository _getInnmeldingDetaljerOversiktInnmelding;

        public OppdatereInnmeldingController(IInnmeldingRepository innmeldingRepository, IGeometriRepository geometriRepository)
        {
            _innmeldingRepository = innmeldingRepository;
            _geometriRepository = geometriRepository;
           // _getInnmeldingDetaljerOversiktInnmelding = innmeldingRepository;
        }
        /*
        public Task<InnmeldingModel> GetInnmeldingDetaljerOversiktInnmelding(int innmeldingId)
        {
            throw new NotImplementedException();
        } */

        [HttpGet("index")]
        public ActionResult Index()
        {
            var viewModel = new OppdatereInnmeldingViewModel
            {
                OppdatereInnmeldinger = _innmeldingRepository.GetInnmeldingAsync().Result.ToList()
            };
            return View(viewModel);
        }
        
        [HttpGet("oppdatere")]
        public async Task<IActionResult> OppdatereInnmelding()
        {
            // Retrieve data from repository
            IEnumerable<InnmeldingModel> innmeldinger = await _innmeldingRepository.GetInnmeldingAsync();

            // Create and populate the view model
            var viewModel = new OppdatereInnmeldingViewModel
            {
                OppdatereInnmeldinger = innmeldinger.ToList()
            };

            // Pass data to the view
            return View(viewModel);
        }
        /*
        public int hardcode = 10;
        [HttpGet("{hardcode}")]
        public async Task<IActionResult> OppdatereInnmelding()
        {
            // Retrieve data from repository
            IEnumerable<InnmeldingModel> innmeldinger = await _innmeldingRepository.GetInnmeldingAsync();
            
            var innmelding = await _getInnmeldingDetaljerOversiktInnmelding.GetOppdatereInnmeldingByIdAsync(hardcode);

            if (innmelding == null)
            {
                return RedirectToAction("MineInnmeldinger", "MineInnmeldinger");
            }
            

            var geometri = await _geometriRepository.GetGeometriByInnmeldingIdAsync(hardcode);

            var viewModel = new OppdatereInnmeldingViewModel
            {
                OppdatereInnmeldinger = innmeldinger.ToList(),
                InnmeldingId = innmelding,
                Geometri = geometri
            };

            return View(viewModel);
        }*/
    }
}