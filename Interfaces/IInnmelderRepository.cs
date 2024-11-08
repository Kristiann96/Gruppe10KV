using System.Threading.Tasks;
using Models.Entities;

namespace Interface
{
    public interface IInnmelderRepository
    {
        Task<bool> ErGyldigInnmelderEpost(string epost);
        Task<InnmelderModel?> HentInnmelderMedEpost(string epost);

        // hente innmelderdata når de er logget inn
        Task<InnmelderModel?> HentInnmelding(int personId);
    }
}