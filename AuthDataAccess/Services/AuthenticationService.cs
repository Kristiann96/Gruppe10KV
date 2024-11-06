using AuthInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace AuthDataAccess.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> DoesEmailExistAsync(string email)
    {
        var normalizedEmail = _userManager.NormalizeEmail(email);
        var existingUser = await _userManager.FindByEmailAsync(email);
        return existingUser != null;
    }

    public async Task<(bool success, string[] errors)> RegisterInnmelderAsync(string email, string password)
    {
        // Sjekk om epost allerede eksisterer
        if (await DoesEmailExistAsync(email))
        {
            return (false, new[] { "Denne e-postadressen er allerede registrert." });
        }

        var user = new IdentityUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray());

        var roleResult = await _userManager.AddToRoleAsync(user, UserRoles.Innmelder);

        if (!roleResult.Succeeded)
        {
            // Hvis rolletildelingen feiler, prøver vi å slette brukeren
            await _userManager.DeleteAsync(user);
            return (false, roleResult.Errors.Select(e => e.Description).ToArray());
        }

        return (true, Array.Empty<string>());
    }

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

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
        if (user == null)
            return false;

        return await _userManager.IsInRoleAsync(user, role);
    }

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
}