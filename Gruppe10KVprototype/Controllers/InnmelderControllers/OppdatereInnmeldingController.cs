using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Models.Models;
using Interface;
using ViewModels;
using LogicInterfaces;
using Models.Exceptions;
using System.Text.Json;


namespace Gruppe10KVprototype.Controllers.OppdatereInnmelderControllers
{
    [Route("OppdatereInnmelding/[controller]")]
    public class OppdatereInnmeldingController : Controller
    {
        private readonly IGeometriRepository _geometriRepository;
        private readonly IInnmeldingRepository _innmeldingRepository;
        private readonly IInnmeldingOpprettelseLogic _innmeldingOpprettelseLogic;

        public OppdatereInnmeldingController(
            IInnmeldingRepository innmeldingRepository, 
            IGeometriRepository geometriRepository, 
            IInnmeldingOpprettelseLogic innmeldingOpprettelseLogic)
        {
            _innmeldingRepository = innmeldingRepository;
            _geometriRepository = geometriRepository;
            _innmeldingOpprettelseLogic = innmeldingOpprettelseLogic;
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

        //Oppdatere geometri data fra bruker på "OppdatereInnmelding"
        [HttpPost("oppdaterGeo")]
        public async Task<IActionResult> OppdatereInnmeldingGeometri(OppdatereInnmeldingViewModel viewModel)
        {
            // Validate the input data
            if (viewModel.InnmeldingId <= 0 || string.IsNullOrEmpty(viewModel.GeometriGeoJson))
            {
                // Handle invalid input
                return BadRequest("Invalid input data.");
            }

            try
            {
                var geometri = new Geometri
                {
                    GeometriGeoJson = viewModel.GeometriGeoJson
                };

                // Validate only
                await _innmeldingOpprettelseLogic.BareValidereGeometriData(geometri);

                // If validation passes, update the database
                var oppdatertGeometri = await _geometriRepository.OppdatereInnmeldingGeometriAsync(
                    viewModel.InnmeldingId,
                    viewModel.GeometriGeoJson);

                if (oppdatertGeometri != null)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Geometri ble oppdatert",
                        redirectUrl = Url.Action("OppdatereInnmelding", new { id = viewModel.InnmeldingId })
                    });
                }
                else
                {
                    return StatusCode(500, "Kunne ikke oppdatere geometrien.");
                }
            }
            catch (ForretningsRegelExceptionModel ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "En intern feil oppstod: " + ex.Message);
            }

            /* // Parse the GeoJSON to ensure it's valid
            try
            {
                var geoJsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(viewModel.GeometriGeoJson);
                if (geoJsonObject == null)
                {
                    return BadRequest("Invalid GeoJSON data.");
                }
            }
            catch (Exception)
            {
                return BadRequest("Invalid GeoJSON format.");
            }

            try
            {
                // Update the database with the new geometry data
                var oppdatertGeometri = await _geometriRepository.OppdatereInnmeldingGeometriAsync(viewModel.InnmeldingId, viewModel.GeometriGeoJson);
                if (oppdatertGeometri != null)
                {
                    // Provide feedback to the user
                    return Ok("Geometry updated successfully.");
                }
                else
                {
                    return StatusCode(500, "An error occurred while updating the geometry.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal server error occurred.");
            }*/
        }
    }
}