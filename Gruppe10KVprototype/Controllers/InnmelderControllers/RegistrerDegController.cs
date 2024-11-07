using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;
public class RegistrerDegController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public RegistrerDegController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult RegistrerDeg()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult RegistrerInnmelder(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View("RegistrerDeg");
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrerInnmelder(BrukerregistreringViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        return RedirectToLocal(returnUrl);
    }
    public IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction(nameof(RegistrerDeg));
        }
    }
}
