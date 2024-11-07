using Models.Models;

namespace LogicInterfaces
{
    public interface IKommuneAPILogic
    {
        Task<List<Kommune>> GetKommunerAsync();

        Task<Kommune> GetKommuneByCoordinatesAsync(double lat, double lng);
        Task<string> GetKommuneStringFromGeometri(Geometri geometri);
    }
}


