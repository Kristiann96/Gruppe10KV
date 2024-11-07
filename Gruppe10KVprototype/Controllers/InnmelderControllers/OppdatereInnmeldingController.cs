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
using Microsoft.EntityFrameworkCore;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    [Route("OppdatereInnmelding/[controller]")]
    public class OppdatereInnmeldingController : Controller
    {
        private readonly IInnmeldingRepository _innmeldingRepository;
        private readonly IGeometriRepository _geometriRepository;
        private readonly IInnmeldingRepository _getOppdatereInnmeldingByIdAsync;

        public OppdatereInnmeldingController(IInnmeldingRepository innmeldingRepository, IGeometriRepository geometriRepository)
        {
            _innmeldingRepository = innmeldingRepository;
            _geometriRepository = geometriRepository;
            _getOppdatereInnmeldingByIdAsync = innmeldingRepository;
        }

        [HttpGet("index")]
        public ActionResult Index(int innmeldingIdUpdate)
        {
            var viewModel = new OppdatereInnmeldingViewModel
            {
                OppdatereInnmeldinger = _innmeldingRepository.GetInnmeldingAsync(innmeldingIdUpdate).Result.ToList()
            };
            return View(viewModel);
        }

        
        [HttpGet("oppdatere")]
        public async Task<IActionResult> OppdatereInnmelding(int id)
        {
            // Retrieve data from repository
            IEnumerable<InnmeldingModel> innmeldinger = await _innmeldingRepository.GetInnmeldingAsync(id);
            
            if (innmeldinger == null)
            {
                return RedirectToAction("MineInnmeldinger", "MineInnmeldinger");
            }

            var geometri = await _geometriRepository.GetGeometriOppdatereInnmelding(id);

            // Create and populate the view model
            var viewModel = new OppdatereInnmeldingViewModel
            {
                OppdatereInnmeldinger = innmeldinger.ToList(),
                InnmeldingId = id,
                Geometri = geometri
            };

            // Pass data to the view
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> OppdatereInnmeldingForm(OppdatereInnmeldingViewModel viewModel)
        {
            // Fetch the existing record from the repository
            var innmelding = (await _innmeldingRepository.GetInnmeldingAsync(viewModel.InnmeldingId)).FirstOrDefault();
            if (innmelding == null)
            {
                return NotFound();
            }

            // Update the record with new values
            innmelding.Tittel = viewModel.Tittel;
            innmelding.Beskrivelse = viewModel.Beskrivelse;

            // Save changes to the repository
            await _innmeldingRepository.OppdatereInnmeldingFormAsync(innmelding);

            // Redirect back to the update page or another appropriate page
            return RedirectToAction("OppdatereInnmelding", new { id = viewModel.InnmeldingId });
        }
    }
}