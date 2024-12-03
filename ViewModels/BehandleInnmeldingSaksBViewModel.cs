using System.Collections;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Models.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace ViewModels;

public class BehandleInnmeldingSaksBViewModel
{
    public int InnmeldingId { get; set; }
    public string Tittel { get; set; } = null!;
    public string Beskrivelse { get; set; } = null!;
    public string Status { get; set; }
    public string Prioritet { get; set; }
    public string KartType { get; set; }
    public string Fornavn { get; set; } = null!;
    public string Etternavn { get; set; } = null!;
    public string Telefonnummer { get; set; } = null!;
    public int? InnmelderId { get; set; }
    public string InnmelderType { get; set; } = null!;
    public string DisplayInnmelderType { get; set; }
    public int? SaksbehandlerId { get; set; }
    public string Stilling { get; set; } = null!;
    public string Jobbepost { get; set; } = null!;
    public string Jobbtelefon { get; set; } = null!;
    public int? GjestInnmelderId { get; set; }
    public string GeometriGeoJson { get; set; }
    public int? ValgtSaksbehandlerId { get; set; }
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

    public class ViewEnumOption
    {
        public string Value { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }

    public IEnumerable<ViewEnumOption> StatusEnums { get; set; } = new List<ViewEnumOption>();
    public IEnumerable<ViewEnumOption> PrioritetEnums { get; set; } = new List<ViewEnumOption>();
    public IEnumerable<ViewEnumOption> KartTypeEnums { get; set; } = new List<ViewEnumOption>();
    public IEnumerable<ViewEnumOption> InnmelderTypeEnums { get; set; } = new List<ViewEnumOption>();
}






