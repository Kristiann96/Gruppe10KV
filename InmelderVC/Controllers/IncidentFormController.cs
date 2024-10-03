using BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace InmelderVC.Controllers;

public class IncidentFormController : Controller
{
    private readonly IncidentFormService _service;

    public IncidentFormController(IncidentFormService service)
    {
        _service = service;
    }
    // Viser skjemaet
    public IActionResult Form()
    {
        return View("Form");
    }

    // Tar imot skjemaet og lagrer det i databasen
    [HttpPost]
    public async Task<IActionResult> SubmitForm(IncidentFormModel model)
    {
        if (ModelState.IsValid)
        {
            await _service.SubmitIncidentFormAsync(model);  // Lagrer skjemaet i databasen
            return View("FormResult", model);          // Viser resultatet
        }
        return View("Form", model);  // Viser skjemaet igjen hvis validering feiler
    }

    // Viser resultatet etter skjemaet er sendt inn
    public IActionResult FormResult(IncidentFormModel model)
    {
        return View(model);
    }
}
