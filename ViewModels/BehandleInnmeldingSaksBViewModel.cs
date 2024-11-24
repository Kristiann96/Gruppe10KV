using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Models.Models;
using System.Collections.Generic;

namespace ViewModels;

public class BehandleInnmeldingSaksBViewModel
{
    public InnmeldingModel InnmeldingModel { get; set; }
    public Geometri Geometri { get; set; }
    public List<SelectListItem> StatusOptions { get; set; }
    public List<SelectListItem> PrioritetOptions { get; set; }
    public List<SelectListItem> KartTypeOptions { get; set; }
    public PersonModel PersonModel { get; set; }
    public InnmelderModel InnmelderModel { get; set; }
    public SaksbehandlerModel SaksbehandlerModel { get; set; }
    public int? ValgtSaksbehandlerId { get; set; }
    public List<SelectListItem> InnmelderTypeOptions { get; set; }

    public List<(SaksbehandlerModel, PersonModel)> SaksbehandlereMedPerson { get; set; }

    public IEnumerable<SelectListItem> ValgbareSaksbehandlere
    {
        get
        {
            if (SaksbehandlereMedPerson == null) return Enumerable.Empty<SelectListItem>();

            return SaksbehandlereMedPerson
                .Where(sb => sb.Item1 != null && sb.Item2 != null)
                .Select(sb => new SelectListItem
                {
                    Text = $"{sb.Item2.Fornavn} {sb.Item2.Etternavn}",
                    Value = sb.Item1.SaksbehandlerId.ToString(),
                    Selected = sb.Item1.SaksbehandlerId == ValgtSaksbehandlerId
                });
        }
    }
}


