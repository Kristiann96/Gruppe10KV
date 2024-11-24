﻿using Interface;
using Microsoft.AspNetCore.Mvc;
using ViewModels;
using System.Threading.Tasks;
using Models.Entities;
using Models.Models;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    public class BidraTilKartForbedringController : Controller
    {
        private readonly IGeometriRepository _geometriRepository;
        private readonly IVurderingRepository _vurderingRepository;

        public BidraTilKartForbedringController(
            IGeometriRepository geometriRepository,
            IVurderingRepository vurderingRepository)
        {
            _geometriRepository = geometriRepository;
            _vurderingRepository = vurderingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> BidraTilKartForbedring()
        {
            var geometriData = await _geometriRepository.GetAktiveGeometriMedInnmeldingAsync();

            var viewModel = new BidraTilKartForbedringViewModel
            {
                GeometriData = geometriData
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> LagreVurdering([FromBody] VurderingModel vurdering)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _vurderingRepository.LeggTilVurderingAsync(vurdering);
                return Json(new JsonResponseModel() { Success = true, Message = "Takk for ditt bidrag!" });

            }
            catch
            {
                return StatusCode(500, "Det oppstod en feil ved lagring av vurderingen.");
            }
        }
    }
}
