using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Models;

namespace Interface
{
    public interface IGeometriRepository
    {
        // Henter alle geometriobjekter
        Task<IEnumerable<Geometri>> GetAllGeometriAsync();

        // Henter spesifikk geometri basert på innmelding_id
        Task<Geometri> GetGeometriByInnmeldingIdAsync(int innmeldingId);



        //Lagrer et geometriobjekt

        Task<bool> LagreGeometriAsync(Geometri geometri);
    }
}

