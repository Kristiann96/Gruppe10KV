using Gruppe10KVprototype.Models;
using Microsoft.AspNetCore.Mvc;
using Models.InnmelderModels;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;

public class InnmelderLoginController : Controller
{
    // GET: /UserPages/Login
    [HttpGet]
    public IActionResult Login()
    {
        // Specify the full view path if necessary
        return View("~/Views/UserPages/Login.cshtml", new InnmeldingLoginModel());
    }

    // POST: /UserPages/Login
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
        return View("~/Views/UserPages/Login.cshtml", model);
    }
}