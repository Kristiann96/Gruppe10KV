using Interface;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;

public class InnmelderSkjemaIncidentFormController : Controller
{
    private readonly IIncidentFormRepository _repository;

    public InnmelderSkjemaIncidentFormController(IIncidentFormRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Form(string geoJson)
    {
        var model = new IncidentFormModel
        {
            GeoJson = geoJson,
            Description = ""
        };
        return View("InnmelderSkjemaIncidentFormView", model);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitForm(IncidentFormModel form)
    {
        if (ModelState.IsValid)
        {
            await _repository.SaveIncidentFormAsync(form);
            return View("InnmelderSkjemaIncidentFormResultat", form);
        }

        return View("InnmelderSkjemaIncidentFormView", form);
    }

    public async Task<IActionResult> FormResult(int id)
    {
        var form = await _repository.GetIncidentByIdAsync(id);
        return View("InnmelderSkjemaIncidentFormResultat", form);
    }
}