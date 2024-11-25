using System.Collections.Generic;
using System.Threading.Tasks;

using Models.Entities;

namespace Interface
{
    public interface IInnmeldingRepository
    {
        Task<IEnumerable<InnmeldingModel>> GetInnmeldingAsync(int innmeldingIdUpdate);
        Task<IEnumerable<InnmeldingModel>> HentInnmeldingerFraInnmelderIdAsync(int innmeldingID);
        Task<string> GetStatusEnumValuesAsync();
        Task<string> GetPrioritetEnumValuesAsync();
        Task<string> GetKartTypeEnumValuesAsync();
        Task<string> GetInnmelderTypeEnumValuesAsync();
        Task<bool> OppdatereEnumSaksBAsync(int innmeldingId, InnmeldingModel model);
        Task<bool> OppdaterSaksbehandler(int innmeldingId, int? saksbehandlerId);
        Task<bool> OppdaterInnmelderType(int innmelderId, InnmeldingModel model);
        Task<bool> OppdatereTittelOgBeskrivelsePaaInnmeldingAsync(InnmeldingModel innmelding);
        Task<IEnumerable<InnmeldingModel>> HentInnmeldingerFraInnmelderIdAsync(string userEmail);
    }
}