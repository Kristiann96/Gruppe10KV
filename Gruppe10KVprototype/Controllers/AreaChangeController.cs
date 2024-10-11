using Gruppe10KVprototype.Models;
using Microsoft.AspNetCore.Mvc;
using Models.SaksbehandlerModels;

namespace Gruppe10KVprototype.Controllers;

public class AreaChangeController : Controller
{
    private static List<AreaChangeModel> changes = new List<AreaChangeModel>();
    
    //Handle form submission to register a new change
    [HttpGet]
    public IActionResult RegisterAreaChange()
    {
        return View();
    }
    
    //Handle form submission to register a new change
    [HttpPost]
    public IActionResult RegisterAreaChange(string geoJson, string description)
    {
        var newChange = new AreaChangeModel
        {
            Id = Guid.NewGuid().ToString(),
            GeoJson = geoJson,
            Description = description
        };
        
        //Save the change in the static in-memory list
        changes.Add(newChange);
        
        //Redirect to the overview of changes
        return RedirectToAction("AreaChangeOverview");
    }

    //Display the overview of registered changes
    [HttpGet]
    public IActionResult AreaChangeOverview()
    {
        return View(changes);
    }
}