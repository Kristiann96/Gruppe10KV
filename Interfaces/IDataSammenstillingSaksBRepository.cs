using Models.Entities;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IDataSammenstillingSaksBRepository
    {
        Task<(InnmeldingModel, PersonModel, InnmelderModel, SaksbehandlerModel)> GetInnmeldingMedDetaljerAsync(
            int innmeldingId);

        Task<(IEnumerable<(InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel)> Data, int TotalPages)>
            GetOversiktAlleInnmeldingerSaksBAsync(int pageNumber, int pageSize, string searchTerm);

        
          
    }
}