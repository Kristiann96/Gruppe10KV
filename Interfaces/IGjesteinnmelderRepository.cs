using System.Threading.Tasks;
using Models.Entities;

namespace Interface
{
    public interface IGjesteinnmelderRepository
    {
        Task<int?> HentGjesteinnmelderIdAsync(string? gjesteinnmelderEpost);
    }
}