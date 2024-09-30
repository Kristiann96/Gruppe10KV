using Microsoft.AspNetCore.Mvc;

namespace SaksbehandlerProject.Views.Shared
{
    public class BoxViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
