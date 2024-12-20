﻿using System.Security.Claims;
using AuthInterface;
using Interface;
using LogicInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using ViewModels;

namespace Gruppe10KVprototype.Controllers.InnmelderControllers;

[Authorize(Roles = UserRoles.Innmelder)]
[AutoValidateAntiforgeryToken]
public class MineInnmeldingerController : Controller
{
    private readonly IInnmeldingRepository _innmeldingRepository;
    private readonly IEnumLogic _enumLogic;

    public MineInnmeldingerController(IInnmeldingRepository innmeldingRepository, IEnumLogic enumLogic)
    {
        _innmeldingRepository = innmeldingRepository;
        _enumLogic = enumLogic;
    }

    [HttpGet]
    public async Task<IActionResult> MineInnmeldinger(int pageNumber = 1, int pageSize = 10, string sortColumn = "InnmeldingId", string sortOrder = "asc")
    {
        try
        {
           
            var innmeldinger = await _innmeldingRepository.HentInnmeldingerFraBrukerAsync(User);

           
            foreach (var innmelding in innmeldinger)
            {
                innmelding.Status = _enumLogic.ConvertToDisplayFormat(innmelding.Status);
            }

            innmeldinger = SortInnmeldinger(innmeldinger, sortColumn, sortOrder);

            int totalInnmeldinger = innmeldinger.Count();
            int totalPages = (int)Math.Ceiling(totalInnmeldinger / (double)pageSize);

            var pagedInnmeldinger = innmeldinger
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new MineInnmeldingerViewModel
            {
                Innmeldinger = pagedInnmeldinger,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                CurrentSortColumn = sortColumn,
                CurrentSortOrder = sortOrder
            };

        return View(viewModel);
        }
        catch (Exception)
        {
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