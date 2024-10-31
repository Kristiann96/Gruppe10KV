using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Models.DTOs;
using Models.Models;
using System;
using System.Collections.Generic;

namespace ViewModels
{
    public class BehandleInnmeldingSaksBViewModel
    {
        public InnmeldingDetaljerKartvisningSaksBModel InnmeldingDetaljer { get; set; }
        public Geometri Geometri { get; set; }
        public List<SelectListItem> StatusOptions { get; set; }
    }
}