using Interface;
using ServicesInterfaces;
using LogicInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Models.Models;
using ViewModels;

namespace Services
{
    public class OppdatereInnmeldingService : IOppdatereInnmeldingService
    {
        private readonly IInnmeldingRepository _innmeldingRepository;
        private readonly IGeometriRepository _geometriRepository;
        private readonly ITransaksjonsRepository _transaksjonsRepository;
        private readonly IInnmeldingLogic _innmeldingLogic;

        public OppdatereInnmeldingService(
            IInnmeldingRepository innmeldingRepository,
            IGeometriRepository geometriRepository,
            ITransaksjonsRepository transaksjonsRepository,
            IInnmeldingLogic innmeldingOppdateringLogic)
        {
            _innmeldingRepository = innmeldingRepository;
            _geometriRepository = geometriRepository;
            _transaksjonsRepository = transaksjonsRepository;
            _innmeldingLogic = innmeldingOppdateringLogic;
        }

        public async Task<OppdatereInnmeldingViewModel> HentInnmeldingForOppdateringAsync(int innmeldingId)
        {
            var innmelding = await _innmeldingRepository.GetInnmeldingAsync(innmeldingId);
            if (!innmelding.Any())
            {
                throw new KeyNotFoundException($"Innmelding med id {innmeldingId} ble ikke funnet");
            }

            var geometri = await _geometriRepository.GetGeometriByInnmeldingIdAsync(innmeldingId);

            return new OppdatereInnmeldingViewModel
            {
                InnmeldingId = innmeldingId,
                Tittel = innmelding.First().Tittel,
                Beskrivelse = innmelding.First().Beskrivelse,
                GeometriGeoJson = geometri?.GeometriGeoJson
            };
        }

        public async Task<bool> OppdatereInnmeldingAsync(OppdatereInnmeldingViewModel model)
        {
            var innmelding = new InnmeldingModel
            {
                InnmeldingId = model.InnmeldingId,
                Tittel = model.Tittel,
                Beskrivelse = model.Beskrivelse
            };

            await _innmeldingLogic.ValiderInnmeldingData(innmelding);
            return await _innmeldingRepository.OppdatereInnmeldingAsync(innmelding);
        }

        public async Task<bool> OppdatereGeometriAsync(int innmeldingId, string geometriGeoJson)
        {
            var geometri = new Geometri
            {
                InnmeldingId = innmeldingId,
                GeometriGeoJson = geometriGeoJson
            };

            await _innmeldingLogic.ValidereGeometriDataForOppdatering(innmeldingId, geometri);
            return await _geometriRepository.OppdatereGeometriAsync(innmeldingId, geometriGeoJson);
        }

        public async Task<bool> SlettInnmeldingAsync(int innmeldingId)
        {
            var innmelding = await _innmeldingRepository.GetInnmeldingAsync(innmeldingId);
            if (!innmelding.Any())
            {
                throw new KeyNotFoundException($"Innmelding med id {innmeldingId} ble ikke funnet");
            }

            return await _transaksjonsRepository.SlettInnmeldingMedTilhorendeDataAsync(innmeldingId);
        }
    }
}


