using Microsoft.AspNetCore.Mvc;
using Models.InnmelderModels;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;

public class InnmelderLoginController : Controller
{
    // GET: 
    [HttpGet]
    public IActionResult InnmelderLoginView()
    {
        // Specify the full view path if necessary
        return View("InnmelderLoginView", new InnmeldingLoginModel());
    }

    // POST:
    [HttpPost]
    public IActionResult InnmelderLoginView(InnmeldingLoginModel model)
    {
        if (ModelState.IsValid)
        {
            // Replace with your actual authentication logic
            if (model.Email == "test@example.com" && model.Password == "password")
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Ugyldig e-post eller passord.");
        }

        // Return the model to the same view if validation fails
        return View("InnmelderLoginView", model);
    }
}