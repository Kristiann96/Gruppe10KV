using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Entities;
using Models.Models;

namespace Interface
{
    public interface IGeometriRepository
    {
        // Henter alle geometriobjekter
        Task<IEnumerable<Geometri>> GetAllGeometriAsync();

        // Henter spesifikk geometri basert på innmelding_id
        Task<Geometri> GetGeometriByInnmeldingIdAsync(int innmeldingId);


        //  BidraTilKartForbedring gir til InnmedligModel og Geometri
        Task<IEnumerable<(Geometri Geometri, InnmeldingModel Innmelding)>> GetAktiveGeometriMedInnmeldingAsync();
    }
}

