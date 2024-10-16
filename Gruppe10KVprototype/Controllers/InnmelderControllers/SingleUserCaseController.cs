using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;

public class SingleUserCaseController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}