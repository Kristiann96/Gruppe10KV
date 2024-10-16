using Gruppe10KVprototype.Models;
using Microsoft.AspNetCore.Mvc;
using Models.InnmelderModels;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;

public class InnmelderLoginController : Controller
{
    // GET: /InnmelderViews/Login
    [HttpGet]
    public IActionResult Login()
    {
        // Specify the full view path if necessary
        return View("~/Views/InnmelderViews/Login.cshtml", new InnmeldingLoginModel());
    }

    // POST: /InnmelderViews/Login
    [HttpPost]
    public IActionResult Login(InnmeldingLoginModel model)
    {
        if (ModelState.IsValid)
        {
            // Replace with your actual authentication logic
            if (model.Email == "test@example.com" && model.Password == "password")
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Ugyldig e-post eller passord.");
        }

        // Return the model to the same view if validation fails
        return View("~/Views/InnmelderViews/Login.cshtml", model);
    }
}