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
        private readonly IInnmeldingOpprettelseLogic _innmeldingOpprettelseLogic;

        public OppdatereInnmeldingService(
            IInnmeldingRepository innmeldingRepository,
            IGeometriRepository geometriRepository,
            ITransaksjonsRepository transaksjonsRepository,
            IInnmeldingOpprettelseLogic innmeldingOppdateringLogic)
        {
            _innmeldingRepository = innmeldingRepository;
            _geometriRepository = geometriRepository;
            _transaksjonsRepository = transaksjonsRepository;
            _innmeldingOpprettelseLogic = innmeldingOppdateringLogic;
        }

        public async Task<OppdatereInnmeldingViewModel> HentInnmeldingForOppdateringAsync(int innmeldingId)
        {
            // Henter data fra flere repositories og sammenstiller det
            var innmelding = await _innmeldingRepository.GetInnmeldingAsync(innmeldingId);
            var geometri = await _geometriRepository.GetGeometriByInnmeldingIdAsync(innmeldingId);

            if (innmelding == null || !innmelding.Any())
                throw new KeyNotFoundException($"Innmelding med id {innmeldingId} ble ikke funnet");

            // Mapper til ViewModel
            return new OppdatereInnmeldingViewModel
            {
                InnmeldingId = innmeldingId,
                Tittel = innmelding.FirstOrDefault()?.Tittel,
                Beskrivelse = innmelding.FirstOrDefault()?.Beskrivelse,
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

            
            await _innmeldingOpprettelseLogic.ValiderInnmeldingData(innmelding);

            return await _innmeldingRepository.OppdatereInnmeldingAsync(innmelding);
        }


        public async Task<bool> OppdatereGeometriAsync(int innmeldingId, string geometriGeoJson)
        {
            // Validerer geometri
            var geometri = new Geometri
            {
                InnmeldingId = innmeldingId,
                GeometriGeoJson = geometriGeoJson
            };

            await _innmeldingOpprettelseLogic.ValidereGeometriDataForOppdatering(innmeldingId, geometri);

            // Utfører oppdatering
            return await _geometriRepository.OppdatereGeometriAsync(innmeldingId, geometriGeoJson);
        }


        public async Task<bool> SlettInnmeldingAsync(int innmeldingId)
        {
            // Sjekker først om innmeldingen eksisterer
            var innmelding = await _innmeldingRepository.GetInnmeldingAsync(innmeldingId);
            if (!innmelding.Any())
            {
                throw new KeyNotFoundException($"Innmelding med id {innmeldingId} ble ikke funnet");
            }

            // Utfører sletting via transaksjonsRepository
            return await _transaksjonsRepository.SlettInnmeldingMedTilhorendeDataAsync(innmeldingId);
        }
    }
}


