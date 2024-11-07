using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthInterface
{
    public static class UserRoles
    {
        public const string Innmelder = "Innmelder";
        public const string Saksbehandler = "Saksbehandler";
    }

    public interface IAuthService
    {
        // Epost-sjekk
        Task<bool> DoesEmailExistAsync(string email);

        // Registrering
        Task<(bool success, string[] errors)> RegisterInnmelderAsync(string email, string password, int personId);

        // Innlogging/utlogging
        Task<(bool success, string[] errors)> LoginAsync(string email, string password);
        Task LogoutAsync();

        // Rollesjekk
        Task<bool> IsInRoleAsync(string role);

        // Passord reset
        Task<(bool success, string[] errors)> ResetPasswordAsync(string email, string newPassword);

        // Person ID henting
        Task<int?> GetPersonId(ClaimsPrincipal user);
    }
}