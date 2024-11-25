using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Models.Models;
using System.Collections.Generic;

namespace ViewModels;

public class BehandleInnmeldingSaksBViewModel
{

    public int InnmeldingId { get; set; }
    public string Tittel { get; set; } = null!;
    public string Beskrivelse { get; set; } = null!;

    public string Status { get; set; }
    public string Prioritet { get; set; }
    public string KartType { get; set; }

    // Person-detaljer for innmelderen
    public string Fornavn { get; set; } = null!;
    public string Etternavn { get; set; } = null!;
    public string Telefonnummer { get; set; } = null!;

    // Innmelder-detaljer
    public int InnmelderId { get; set; }
    public string InnmelderType { get; set; } = null!;

    // Saksbehandler-detaljer
    public string SaksbehandlerStilling { get; set; } = null!;
    public string SaksbehandlerJobbepost { get; set; } = null!;
    public string SaksbehandlerJobbtelefon { get; set; } = null!;

    public int GjestInnmelderId { get; set; } 



    public string GeometriGeoJson { get; set; }
    public List<SelectListItem> StatusOptions { get; set; }
    public List<SelectListItem> PrioritetOptions { get; set; }
    public List<SelectListItem> KartTypeOptions { get; set; }


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


