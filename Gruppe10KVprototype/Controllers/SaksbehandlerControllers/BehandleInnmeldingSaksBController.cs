using Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using ViewModels;
using Models.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Interface;
using Interfaces;
using LogicInterfaces;
using Models.Entities;
using AuthInterface;
using Microsoft.AspNetCore.Authorization;


namespace Gruppe10KVprototype.Controllers.SaksbehandlerControllers

{
    [Authorize(Roles = UserRoles.Saksbehandler)]

    [AutoValidateAntiforgeryToken]

    public class BehandleInnmeldingSaksBController : Controller
    {
        private readonly IGeometriRepository _geometriRepository;
        private readonly IEnumLogic _enumLogic;
        private readonly IDataSammenstillingSaksBRepository _dataSammenstillingSaksBRepository;
        private readonly IInnmeldingRepository _innmeldingRepository;
        private readonly ISaksbehandlerRepository _saksbehandlerRepository;

        public BehandleInnmeldingSaksBController(
            IGeometriRepository geometriRepository,
            IEnumLogic enumLogic,
            IDataSammenstillingSaksBRepository dataSammenstillingSaksBRepository,
            IInnmeldingRepository innmeldingRepository,
            ISaksbehandlerRepository saksbehandlerRepository)
        {
            _geometriRepository = geometriRepository;
            _enumLogic = enumLogic;
            _dataSammenstillingSaksBRepository = dataSammenstillingSaksBRepository;
            _innmeldingRepository = innmeldingRepository;
            _saksbehandlerRepository = saksbehandlerRepository;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> BehandleInnmeldingSaksB(int id)
        {
            try
            {
                var (innmelding, person, innmelder, saksbehandler,gjesteinnmelder) =
                    await _dataSammenstillingSaksBRepository.GetInnmeldingMedDetaljerAsync(id);

                if (innmelding == null)
                {
                    TempData["ErrorMessage"] = "Innmelding ikke funnet";
                    return RedirectToAction("OversiktAlleInnmeldingerSaksB", "OversiktAlleInnmeldingerSaksB");
                }

                var geometri = await _geometriRepository.GetForBehandleInnmedingAsync(id);
                var saksbehandlereMedPerson = await _saksbehandlerRepository.HentAlleSaksbehandlereMedPersonAsync()
                                              ?? new List<(SaksbehandlerModel, PersonModel)>();

                var viewModel = new BehandleInnmeldingSaksBViewModel
                {
                    InnmeldingId = innmelding.InnmeldingId,
                    Tittel = innmelding.Tittel,
                    Beskrivelse = innmelding.Beskrivelse,
                    Status = innmelding.Status,
                    Prioritet = innmelding.Prioritet,
                    KartType = innmelding.KartType,

                    Fornavn = person?.Fornavn,
                    Etternavn = person?.Etternavn,
                    Telefonnummer = person?.Telefonnummer,
                    InnmelderId = innmelder?.InnmelderId?? 0,
                    InnmelderType = innmelder?.InnmelderType,
                    DisplayInnmelderType = _enumLogic.ConvertToDisplayFormat(innmelder?.InnmelderType),
                    SaksbehandlerId = saksbehandler?.SaksbehandlerId ?? 0,
                    Stilling = saksbehandler?.Stilling,
                    Jobbepost = saksbehandler?.Jobbepost,
                    Jobbtelefon = saksbehandler?.Jobbtelefon,
                    ValgtSaksbehandlerId = saksbehandler?.SaksbehandlerId,
                    GjestInnmelderId = gjesteinnmelder?.GjestInnmelderId?? 0,
                    GeometriGeoJson = geometri.GeometriGeoJson,
                    
                    SaksbehandlereMedPerson = saksbehandlereMedPerson,

                    
                    StatusEnums = (await _enumLogic.GetFormattedStatusEnumValuesAsync())
                        .Select(value => new BehandleInnmeldingSaksBViewModel.ViewEnumOption
                        {
                            Value = _enumLogic.ConvertToDbFormat(value),
                            DisplayName = value
                        }),

                    PrioritetEnums = (await _enumLogic.GetFormattedPrioritetEnumValuesAsync())
                        .Select(value => new BehandleInnmeldingSaksBViewModel.ViewEnumOption
                        {
                            Value = _enumLogic.ConvertToDbFormat(value),
                            DisplayName = value
                        }),

                    KartTypeEnums = (await _enumLogic.GetFormattedKartTypeEnumValuesAsync())
                        .Select(value => new BehandleInnmeldingSaksBViewModel.ViewEnumOption
                        {
                            Value = _enumLogic.ConvertToDbFormat(value),
                            DisplayName = value
                        }),

                    InnmelderTypeEnums = (await _enumLogic.GetFormattedInnmelderTypeEnumValuesAsync())
                        .Select(value => new BehandleInnmeldingSaksBViewModel.ViewEnumOption
                        {
                            Value = _enumLogic.ConvertToDbFormat(value),
                            DisplayName = value
                        })
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Det oppstod en feil ved lasting av innmeldingen";
                return RedirectToAction("OversiktAlleInnmeldingerSaksB", "OversiktAlleInnmeldingerSaksB");
            }
        }

        private static async Task<List<SelectListItem>> TryGetEnumOptions(Func<Task<IEnumerable<string>>> getEnumValues)
        {
            try
            {
                var values = await getEnumValues();
                return values?.Select(v => new SelectListItem { Value = v, Text = v }).ToList()
                       ?? new List<SelectListItem>();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }

        [HttpGet("HentSaksbehandlere")]
        public async Task<IActionResult> HentSaksbehandlereForDropdown()
        {
            var saksbehandlereMedPerson = await _saksbehandlerRepository.HentAlleSaksbehandlereMedPersonAsync();

            return Json(saksbehandlereMedPerson.Select(s => new
            {
                Value = s.Item1.SaksbehandlerId.ToString(),
                Text = $"{s.Item2.Fornavn} {s.Item2.Etternavn}"
            }));
        }


        [HttpPost]
        public async Task<IActionResult> OppdateringAvInnmeldingSaksB(int id, BehandleInnmeldingSaksBViewModel viewModel)
        {
            try
            {
                var model = new InnmeldingModel
                {
                    InnmeldingId = id,
                    Status = _enumLogic.ConvertToDbFormat(viewModel.Status),
                    Prioritet = _enumLogic.ConvertToDbFormat(viewModel.Prioritet),
                    KartType = _enumLogic.ConvertToDbFormat(viewModel.KartType)
                };

                var isValid = await ValidateEnumValues(model);
                if (!isValid)
                {
                    return Json(new { success = false, message = "En eller flere verdier er ugyldige" });
                }

                var result = await _innmeldingRepository.OppdatereEnumSaksBAsync(id, model);

                return Json(new
                {
                    success = result,
                    message = result ? "Endringer lagret" : "Kunne ikke lagre endringer"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Det oppstod en feil ved lagring" });
            }
        }

        private async Task<bool> ValidateEnumValues(InnmeldingModel model)
        {
            var isStatusValid = await _enumLogic.ValidateStatusValueAsync(model.Status);
            var isPrioritetValid = await _enumLogic.ValidatePrioritetValueAsync(model.Prioritet);
            var isKartTypeValid = await _enumLogic.ValidateKartTypeValueAsync(model.KartType);

            return isStatusValid && isPrioritetValid && isKartTypeValid;
        }

        [HttpPost]
        public async Task<IActionResult> LagreSaksbehandler(int innmeldingId, int? saksbehandlerId)
        {
            try
            {
                var result = await _innmeldingRepository.OppdaterSaksbehandler(innmeldingId, saksbehandlerId);

                return Json(new
                {
                    success = result,
                    message = result ? "Saksbehandler er oppdatert" : "Kunne ikke finne innmeldingen"
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "Kunne ikke oppdatere saksbehandler"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> OppdaterInnmelderType([FromForm] string innmelderType, [FromForm] int innmelderId, [FromForm] int innmeldingId)
        {
            try
            {
               
                var isValid = await _enumLogic.ValidateInnmelderTypeValueAsync(innmelderType);
                if (!isValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Ugyldig innmelder type"
                    });
                }

                var model = new InnmeldingModel { InnmelderType = innmelderType };
                var result = await _innmeldingRepository.OppdaterInnmelderType(innmelderId, model);

                return Json(new
                {
                    success = result,
                    message = result ? "Innmeldertype er oppdatert" : "Kunne ikke finne innmelderen"
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "Kunne ikke oppdatere innmeldertype"
                });
            }
        }
    }
}


