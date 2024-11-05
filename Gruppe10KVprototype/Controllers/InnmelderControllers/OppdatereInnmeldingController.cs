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
using Interface;
using Interfaces;
using DataAccess;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    [Route("OppdatereInnmelding/[controller]")]
    public class OppdatereInnmeldingController : Controller
    {
        private readonly IInnmeldingRepository _innmeldingRepository;
        private readonly IGeometriRepository _geometriRepository;
        private readonly IInnmeldingRepository _getInnmeldingDetaljerOversiktInnmelding;

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
        [HttpGet]
        public IActionResult OppdatereInnmeldingDetailsById(int innmeldingId)
        {
            // Get the single innmelding using your repository
            var innmelding = _innmeldingRepository.GetOppdatereInnmeldingByIdAsync(innmeldingId).Result;

            if (innmelding == null)
                return NotFound();

            // Map to view model
            var viewModel = new OppdatereInnmeldingViewModel
            {
                InnmeldingId = innmelding.InnmeldingId,
                Tittel = innmelding.Tittel,
                Status = innmelding.Status,
                Beskrivelse = innmelding.Beskrivelse,
                Geometri = innmelding.Geometri
            };

            return View(viewModel);
        }*/

        [HttpGet("{innmeldingId}")]
        public async Task<IActionResult> OppdatereInnmeldingGeo(int innmeldingId)
        {
            var innmelding = await _getInnmeldingDetaljerOversiktInnmelding.GetOppdatereInnmeldingByIdAsync(innmeldingId);

            if (innmelding == null)
            {
                return NotFound();
            }
            

            var geometri = await _geometriRepository.GetGeometriByInnmeldingIdAsync(innmeldingId);

            var viewModel = new OppdatereInnmeldingViewModel
            {
                InnmeldingId = innmelding,
                Geometri = geometri
            };

            return View(viewModel);
        }
    }
}