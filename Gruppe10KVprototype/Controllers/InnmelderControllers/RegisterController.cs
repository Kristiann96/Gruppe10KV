using Gruppe10KVprototype.Models;
using Microsoft.AspNetCore.Mvc;
using Models.InnmelderModels;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;

public class RegisterController : Controller
{
    [HttpGet]
    public IActionResult Register()
    {
        return View("~/Views/UserPages/Register.cshtml", new InnmelderRegisterModel());
    }


    [HttpPost]
    public IActionResult Register(InnmelderRegisterModel model)
    {
        if (ModelState.IsValid)
        {
            // Example registration logic
            // Replace this with your actual user registration service
            // e.g., await _userService.RegisterUserAsync(model);

            // Simulating user registration
            var registrationSuccessful = RegisterUser(model);

            if (registrationSuccessful)
                // Redirect to a confirmation page or login page
                return RedirectToAction("Login", "InnmelderLogin");
            ModelState.AddModelError("", "Brukernavn eller e-post er allerede i bruk.");
        }

        // Return the model to the same view if validation fails
        return View("~/Views/UserPages/Register.cshtml", model);
    }

    private static bool RegisterUser(InnmelderRegisterModel model)
    {
        // Implement your user registration logic here
        // Return true if successful, false otherwise
        return true; // Change as needed
    }
}