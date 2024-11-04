﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Models.Entities;

namespace Interface
{
    public interface IInnmeldingRepository
    {
        //HENTING AV DATA
        
        //Daniel's interface for "OppdatereInnmelding"
        Task<IEnumerable<InnmeldingModel>> GetInnmeldingAsync();

        Task<IEnumerable<InnmeldingModel>> GetOversiktAlleInnmeldingerSaksBAsync(int pageNumber, int pageSize, string searchTerm);
        Task<int> GetTotalInnmeldingerTellerSaksBAsync(string searchTerm);

        Task<IEnumerable<InnmeldingModel>> HentInnmeldingerFraInnmelderIdAsync(int innmeldingID);

        Task<string> GetStatusEnumValuesAsync();

        //LAGRING AV DATA

        Task<int> LagreInnmeldingAsync(InnmeldingModel innmelding);



    }
}