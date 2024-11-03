using Interface;
using Microsoft.AspNetCore.Mvc;
using ViewModels;
using System.Threading.Tasks;
using System.Linq;


namespace Gruppe10KVprototype.Controllers.InnmelderControllers
{
    public class BidraTilKartForbedringController : Controller
    {
        private readonly IGeometriRepository _geometriRepository;

        public BidraTilKartForbedringController(
            IGeometriRepository geometriRepository)
        {
            _geometriRepository = geometriRepository;
        }

        [HttpGet]
        public async Task<IActionResult> BidraTilKartForbedring()
        {
            var geometriData = await _geometriRepository.GetAktiveGeometriMedInnmeldingAsync();

            var viewModel = new BidraTilKartForbedringViewModel
            {
                GeometriData = geometriData
            };

            return View(viewModel);
        }

    }
}
