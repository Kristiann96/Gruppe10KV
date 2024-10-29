using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs;
using Models.Entities;

namespace Interface
{
    public interface IInnmeldingRepository
    {
        Task<InnmeldingModel> GetInnmeldingByIdAsync(int innmeldingId);
        Task<InnmeldingDetaljKartvisningSaksBModel> GetInnmeldingDetaljerByIdAsync(int innmeldingId);


        Task<IEnumerable<InnmeldingModel>> GetInnmeldingAsync();
    }
}