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
    public IEnumerable<SelectListItem> SaksbehandlerOptions { get; set; }
    public int? ValgtSaksbehandlerId { get; set; }
    public IEnumerable<SelectListItem> InnmelderType { get; set; }
    public List<SelectListItem> InnmelderTypeOptions { get; set; }
}


