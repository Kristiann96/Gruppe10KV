using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;

public class InnmelderSkjemaController : Controller
{
    private readonly IInnmelderRepository _repository;

    public InnmelderSkjemaController(IInnmelderRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Form(string geoJson)
    {
        var model = new InnmelderIncidentFormModel { GeoJson = geoJson };
        return View("InnmelderSkjemaView", model);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitForm(InnmelderIncidentFormModel form)
    {
        if (ModelState.IsValid)
        {
            await _repository.SaveIncidentFormAsync(form);
            return View("InnmelderSkjemaResultat", form);
        }

        return View("InnmelderSkjemaView", form);
    }

    public async Task<IActionResult> FormResult(int id)
    {
        var form = await _repository.GetIncidentByIdAsync(id);
        return View("InnmelderSkjemaResultat", form);
    }
}