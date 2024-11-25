using System.Threading.Tasks;
using Models.Entities;

namespace Interface
{
    public interface IInnmelderRepository
    {
        Task<bool> ErGyldigInnmelderEpost(string epost);
        Task<InnmelderModel?> HentInnmelderMedEpost(string epost);
        Task<InnmelderModel?> HentInnmelding(int personId);
        Task<int> HentInnmelderIdMedEpost(string epost);
    }
}