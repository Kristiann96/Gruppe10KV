using Models.Entities;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface ITransaksjonsRepository
    {
        Task<bool> LagreKomplettInnmeldingAsync(
            int? gjestInnmelderId,
            InnmeldingModel innmelding,
            Geometri geometri);

        void LagreKomplettInnmeldingAsync(
            string epost, 
            InnmeldingModel innmelding, 
            Geometri geometri);

        public Task<bool> LagreKomplettInnmeldingInnloggetAsync(
            string? epost,
            InnmeldingModel innmelding,
            Geometri geometri);
        Task<int> OpprettGjesteinnmelderAsync(string epost);


       Task<(bool success, int personId, string? errorMessage)> OpprettPersonOgInnmelder(
            string fornavn,
            string etternavn,
            string telefonnummer,
            string epost);
        
        Task<bool> SlettInnmeldingMedTilhorendeDataAsync(int innmeldingId);
        
    }




}
