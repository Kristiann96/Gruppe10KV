using AuthInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AuthDataAccess.Services;
using Microsoft.Extensions.Logging;


namespace AuthDataAccess.Services;

/// <summary>
/// Implementerer autentisering og autorisasjonstjenester ved hjelp av ASP.NET Core Identity
/// </summary>
public class AuthService : IAuthService
{
    private const string PersonIdClaimType = "PersonId";
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Sjekker om en epostadresse allerede er registrert i Identity systemet
    /// </summary>
    /// <param name="email">Epostadressen som skal sjekkes</param>
    /// <returns>True hvis eposten eksisterer, ellers false</returns>
    public async Task<bool> DoesEmailExistAsync(string email)
    {
        var normalizedEmail = _userManager.NormalizeEmail(email);
        var existingUser = await _userManager.FindByEmailAsync(email);
        return existingUser != null;
    }

    /// <summary>
    /// Registrerer en ny innmelder i Identity systemet
    /// </summary>
    /// <param name="email">Epostadresse for den nye brukeren</param>
    /// <param name="password">Ønsket passord</param>
    /// <param name="personId">Person ID fra eksisterende database</param>
    /// <returns>Success/failure status og eventuelle feilmeldinger</returns>
    public async Task<(bool success, string[] errors)> RegisterInnmelderAsync(string email, string password, int personId)
    {
        try
        {
            _logger.LogInformation($"Starter registreringsprosess for bruker med epost: {email}");

            // Sjekk først om epost allerede eksisterer i Identity-systemet
            if (await DoesEmailExistAsync(email))
            {
                _logger.LogWarning($"Registrering avbrutt: E-post {email} er allerede registrert i Identity");
                return (false, new[] { "Denne e-postadressen er allerede registrert." });
            }

            // Opprett ny Identity-bruker med epost som både username og email
            _logger.LogInformation($"Oppretter ny Identity-bruker for: {email}");
            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true  // Setter denne til true siden vi vet at eposten er gyldig fra dapper
            };

            // Forsøk å opprette brukeren med passord
            _logger.LogInformation("Forsøker å opprette bruker med passord");
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                _logger.LogError($"Feil ved opprettelse av bruker: {string.Join(", ", errors)}");
                return (false, errors);
            }

            // Legg til PersonId som en claim for å koble mot eksisterende database
            _logger.LogInformation($"Bruker opprettet. Legger til PersonId claim: {personId}");
            var claimResult = await _userManager.AddClaimAsync(user, new Claim(PersonIdClaimType, personId.ToString()));

            if (!claimResult.Succeeded)
            {
                _logger.LogError($"Feil ved tillegg av PersonId claim. Sletter bruker.");
                await _userManager.DeleteAsync(user);
                return (false, claimResult.Errors.Select(e => e.Description).ToArray());
            }

            // Tildel innmelderrolle til brukeren
            _logger.LogInformation($"Legger til Innmelder-rolle for bruker: {email}");
            var roleResult = await _userManager.AddToRoleAsync(user, UserRoles.Innmelder);

            if (!roleResult.Succeeded)
            {
                _logger.LogError($"Feil ved tildeling av rolle. Sletter bruker. Feil: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                // Hvis rolletildeling feiler, slett brukeren for å unngå inkonsistent tilstand
                await _userManager.DeleteAsync(user);
                return (false, roleResult.Errors.Select(e => e.Description).ToArray());
            }

            _logger.LogInformation($"Bruker {email} er nå fullstendig registrert med PersonId {personId}");
            return (true, Array.Empty<string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Uventet feil under registrering av bruker: {email}");
            return (false, new[] { "Det oppstod en uventet feil under registrering. Vennligst prøv igjen senere." });
        }
    }
    /// <summary>
    /// Håndterer innlogging av bruker
    /// </summary>
    /// <param name="email">Brukerens epostadresse</param>
    /// <param name="password">Brukerens passord</param>
    /// <returns>Success/failure status og eventuelle feilmeldinger</returns>
    public async Task<(bool success, string[] errors)> LoginAsync(string email, string password)
    {
        // Sjekk først om epost eksisterer
        if (!await DoesEmailExistAsync(email))
        {
            return (false, new[] { "Ugyldig brukernavn eller passord." });
        }

        var result = await _signInManager.PasswordSignInAsync(email, password,
            isPersistent: false, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return (false, new[] { "Ugyldig brukernavn eller passord." });
        }

        return (true, Array.Empty<string>());
    }

    /// <summary>
    /// Logger ut nåværende bruker
    /// </summary>
    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    /// <summary>
    /// Sjekker om nåværende bruker har en spesifikk rolle
    /// </summary>
    /// <param name="role">Rollen som skal sjekkes</param>
    /// <returns>True hvis brukeren har rollen, ellers false</returns>
    public async Task<bool> IsInRoleAsync(string role)
    {
        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User
            ?? throw new InvalidOperationException("No user context available"));
        if (user == null)
            return false;

        return await _userManager.IsInRoleAsync(user, role);
    }

    /// <summary>
    /// Tilbakestiller passordet for en bruker (kun for testmiljø)
    /// </summary>
    /// <param name="email">Brukerens epostadresse</param>
    /// <param name="newPassword">Nytt ønsket passord</param>
    /// <returns>Success/failure status og eventuelle feilmeldinger</returns>
    public async Task<(bool success, string[] errors)> ResetPasswordAsync(string email, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (false, new[] { "Bruker ikke funnet." });
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

        return result.Succeeded
            ? (true, Array.Empty<string>())
            : (false, result.Errors.Select(e => e.Description).ToArray());
    }

    /// <summary>
    /// Henter Person ID fra brukerens claims
    /// </summary>
    /// <param name="user">ClaimsPrincipal for nåværende bruker</param>
    /// <returns>Person ID hvis funnet, ellers null</returns>
    public int? GetPersonId(ClaimsPrincipal user)  
    {
        var personIdClaim = user.FindFirst(PersonIdClaimType);
        if (personIdClaim != null && int.TryParse(personIdClaim.Value, out int personId))
        {
            return personId;
        }
        return null;
    }
}
