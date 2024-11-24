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
            string gjesteEpost,
            InnmeldingModel innmelding,
            Geometri geometri);
    public Task<bool> LagreKomplettInnmeldingInnloggetAsync(
        string epost,
        InnmeldingModel innmelding,
        Geometri geometri);

        //registrer innmelder som preson og innmelder
        Task<(bool success, int personId)> OpprettPersonOgInnmelder(
            string fornavn,
            string etternavn,
            string telefonnummer,
            string epost);

        //fallback ved feil ved opprettelse av registert innmelder
        Task<bool> SlettPersonOgInnmelder(int personId);

        //innmelder sletter egen innmelding
        Task<bool> SlettInnmeldingMedTilhorendeDataAsync(int innmeldingId);
        
    }




}
