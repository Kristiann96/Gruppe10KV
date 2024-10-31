using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs;
using Models.Entities;

namespace Interface
{
    public interface IInnmeldingRepository
    {
        Task<InnmeldingModel> GetInnmeldingByIdAsync(int innmeldingId);

        //Daniel's interface for "OppdatereInnmelding"
        Task<IEnumerable<InnmeldingModel>> GetInnmeldingAsync();


        Task<InnmeldingDetaljerKartvisningSaksBModel> GetInnmeldingDetaljerByIdAsync(int innmeldingId);
        Task<IEnumerable<InnmeldingModel>> GetOversiktAlleInnmeldingerSaksBAsync();

        Task<string> GetStatusEnumValuesAsync();


    }
}