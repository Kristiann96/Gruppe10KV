using System.Security.Claims;
using AuthInterface;
using Interface;
using LogicInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;
[Authorize(Roles = UserRoles.Innmelder)]
public class MineInnmeldingerController : Controller
{
    private readonly IInnmeldingRepository _innmeldingRepository;
    private readonly IEnumLogic _enumLogic;
    private readonly IInnmelderRepository _innmelderRepository;

    public MineInnmeldingerController(IInnmeldingRepository innmeldingRepository, IEnumLogic enumLogic, IInnmelderRepository inmelderRepository)
    {
        _innmeldingRepository = innmeldingRepository;
        _enumLogic = enumLogic;
        _innmelderRepository = inmelderRepository;
    }

    [HttpGet]
    public async Task<IActionResult> MineInnmeldinger(int pageNumber = 1, int pageSize = 10, string sortColumn = "InnmeldingId", string sortOrder = "asc")
    {
        try
        {
            var userEmail = User.Identity?.Name;
            
            // Hent innmeldinger for den aktuelle innmelderen
            IEnumerable<InnmeldingModel> innmeldinger = await _innmeldingRepository.HentInnmeldingerFraInnmelderIdAsync(userEmail);

            foreach (var innmelding in innmeldinger)
            {
                innmelding.Status = _enumLogic.ConvertToDisplayFormat(innmelding.Status);
            }

            // Sort innmeldinger based on sortColumn and sortOrder
            innmeldinger = SortInnmeldinger(innmeldinger, sortColumn, sortOrder);

            // Calculate total pages
            int totalInnmeldinger = innmeldinger.Count();
            int totalPages = (int)Math.Ceiling(totalInnmeldinger / (double)pageSize);

            // Get the innmeldinger for the current page
            var pagedInnmeldinger = innmeldinger
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Create the ViewModel
            var viewModel = new MineInnmeldingerViewModel
            {
                Innmeldinger = pagedInnmeldinger,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                CurrentSortColumn = sortColumn,
                CurrentSortOrder = sortOrder
            };

        // Return the view with the ViewModel
        return View(viewModel);
        }
        catch (Exception ex)
        {
            // Ved uventet feil, redirect til liste med generisk feilmelding
            TempData["ErrorMessage"] = "En feil oppstod ved henting av innmeldinger";
            return RedirectToAction("Error", "Home");
        }
    }

    private IEnumerable<InnmeldingModel> SortInnmeldinger(IEnumerable<InnmeldingModel> innmeldinger, string sortColumn, string sortOrder)
    {
        var propertyInfo = typeof(InnmeldingModel).GetProperty(sortColumn);
        if (propertyInfo == null)
        {
            throw new InvalidOperationException($"Property '{sortColumn}' does not exist on type '{typeof(InnmeldingModel).Name}'");
        }

        return sortOrder == "asc"
            ? innmeldinger.OrderBy(x => propertyInfo.GetValue(x, null))
            : innmeldinger.OrderByDescending(x => propertyInfo.GetValue(x, null));
    }
}