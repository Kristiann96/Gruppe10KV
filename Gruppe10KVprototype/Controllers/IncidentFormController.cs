﻿using Microsoft.AspNetCore.Mvc;
using Interfaces;
using Entities;
using System.Threading.Tasks;

namespace Gruppe10KVprototype.Controllers
{
    public class IncidentFormController : Controller
    {
        private readonly IIncidentFormRepository _repository;

        public IncidentFormController(IIncidentFormRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Form(string geoJson)
        {
            var model = new IncidentForm { GeoJson = geoJson };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitForm(IncidentForm form)
        {
            if (ModelState.IsValid)
            {
                await _repository.SaveIncidentFormAsync(form);
                return View("FormResult", form);
            }
            return View("Form", form);
        }

        public async Task<IActionResult> FormResult(int id)
        {
            var form = await _repository.GetIncidentByIdAsync(id);
            return View(form);
        }
    }
}