using Microsoft.AspNetCore.Mvc;
using SaksbehandlerProject.Controllers;
namespace SaksbehandlerProject.Controllers;

public class SaksbehandlerComCon : Controller
{
    public IActionResult SaksbehandlerIndex()
    {
        HomeController homeController = new HomeController();
        return homeController.Index();
    }
}
