using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Entities;
using Models.Models;

namespace Interface
{
    public interface IGeometriRepository
    {
        Task<IEnumerable<Geometri>> GetAllGeometriAsync();
        Task<Geometri> GetGeometriByInnmeldingIdAsync(int innmeldingId);
        Task<IEnumerable<(Geometri Geometri, InnmeldingModel Innmelding)>> GetAktiveGeometriMedInnmeldingAsync();
        Task<bool> OppdatereGeometriAsync(int innmeldingId, string geometriGeoJson);
        Task<Geometri> GetForBehandleInnmedingAsync(int innmeldingId);

    }
}

