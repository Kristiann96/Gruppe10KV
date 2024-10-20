using Models.Entities;

public interface IInnmeldingERepository
{
    Task<IEnumerable<InnmeldingEModel>> GetAllInnmeldingerAsync();
    Task<InnmeldingEModel> GetInnmeldingByIdAsync(int innmeldID);
    Task<StatusEnum> GetStatusByInnmeldIdAsync(int innmeldID);
    Task<bool> UpdateStatusAsync(int innmeldID, StatusEnum status);
    Task<bool> UpdateInnmeldingAsync(InnmeldingEModel innmeldingE);
    IEnumerable<StatusEnum> GetAvailableStatuses();
}