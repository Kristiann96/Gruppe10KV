using System.Threading.Tasks;
using Models.DTOs;
using Models.Entities;

namespace Interface
{
    public interface IInnmeldingRepository
    {
        Task<Innmelding> GetInnmeldingByIdAsync(int innmeldingId);
        Task<InnmeldingDetaljKartvisningSaksBModel> GetInnmeldingDetaljerByIdAsync(int innmeldingId);
    }
}