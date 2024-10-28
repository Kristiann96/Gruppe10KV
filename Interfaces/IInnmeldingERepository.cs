using Models.Entities;

namespace Interface
{


    public interface IInnmeldingERepository //fra dummy tabell - dette skal slettes etterhvert
    {
        Task<IEnumerable<InnmeldingEModel>> GetAllInnmeldingerAsync();
        Task<InnmeldingEModel> GetInnmeldingByIdAsync(int innmeldID);
        Task<StatusEnum> GetStatusByInnmeldIdAsync(int innmeldID);
        Task<bool> UpdateStatusAsync(int innmeldID, StatusEnum status);
        Task<bool> UpdateInnmeldingAsync(InnmeldingEModel innmeldingE);
        IEnumerable<StatusEnum> GetAvailableStatuses();
    }
}