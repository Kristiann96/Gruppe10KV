using Models.Models;

namespace LogicInterfaces
{
    public interface IKartverketAPILogic
    {
        Task<List<Kommune>> GetKommunerAsync();

        Task<Kommune> GetKommuneByCoordinatesAsync(double lat, double lng);
    }
}


