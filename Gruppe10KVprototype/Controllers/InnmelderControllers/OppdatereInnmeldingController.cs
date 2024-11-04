using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using System.Threading.Tasks;
using Interface;
using System.Collections.Generic;
using ViewModels;
using System.Linq;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    public class OppdatereInnmeldingController : Controller
    {
        private readonly IInnmeldingRepository _innmeldingRepository;
        private readonly IGeometriRepository _geometriRepository;

        public ActionResult Index()
        {
            var viewModel = new OppdatereInnmeldingViewModel
            {
                OppdatereInnmeldinger = _innmeldingRepository.GetInnmeldingAsync().Result.ToList()
            };
            return View(viewModel);
        }

        public OppdatereInnmeldingController(IInnmeldingRepository repository, IGeometriRepository geometriRepository)
        {
            _innmeldingRepository = repository;
            _geometriRepository = geometriRepository;
        }

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
        public IActionResult OppdatereInnmelding(int innmeldingId)
        {
            var innmelding = _context.Innmeldinger
                .Where(i => i.InnmeldingId == innmeldingId)
                .Select(i => new OppdatereInnmeldingViewModel
                {
                    InnmeldingId = i.InnmeldingId,
                    Tittel = i.Tittel,
                    Status = i.Status,
                    Beskrivelse = i.Beskrivelse,
                    GeometriData = i.GeometriData
                })
                .FirstOrDefault();

            return View(innmelding);
        } */

        /*
        public async Task<IActionResult> OppdatereInnmelding(int innmeldingId)
        {
            var innmelding = await _innmeldingRepository.GetInnmeldingAsync()
                .Where(i => i.InnmeldingId == innmeldingId)
                .FirstOrDefaultAsync();

            if (innmelding == null)
            {
                return NotFound();
            }

            var geometri = await _geometriRepository.GetGeometriByInnmeldingIdAsync(innmeldingId);

            var viewModel = new OppdatereInnmeldingViewModel
            {
                InnmeldingId = innmelding.InnmeldingId,
                Tittel = innmelding.Tittel,
                Status = innmelding.Status,
                Beskrivelse = innmelding.Beskrivelse,
                GeometriData = geometri?.GeometriGeoJson // Fetch geometry data
            };

            return View(viewModel);
        } */

        public IActionResult OppdatereInnmelding(int innmeldingId)
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
                GeometriData = innmelding.GeometriData
            };

            return View(viewModel);
        }
    }
}