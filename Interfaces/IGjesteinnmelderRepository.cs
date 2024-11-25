using System.Threading.Tasks;
using Models.Entities;

namespace Interface
{
    public interface IGjesteinnmelderRepository
    {
        Task<int> OpprettGjesteinnmelderAsync(GjesteinnmelderModel gjesteinnmelder);
    }
}