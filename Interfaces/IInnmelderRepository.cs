using System.Threading.Tasks;
using Models.Entities;

namespace Interface
{
    public interface IInnmelderRepository
    {
        Task<Innmelder> GetInnmelderByIdAsync(int innmelderId);
    }
}