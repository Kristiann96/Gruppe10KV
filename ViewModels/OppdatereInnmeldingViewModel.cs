using System.Collections;
using System.Collections.Generic;
using Models.Models;
using Models.Entities;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class OppdatereInnmeldingViewModel
    {
        public int InnmeldingId { get; set; }

        [Required(ErrorMessage = "Tittel ma fylles ut")]
        [StringLength(100, ErrorMessage = "Tittel kan ikke vaere lengre enn 100 tegn")]
        [Display(Name = "Tittel")]
        public string Tittel { get; set; }

        [Required(ErrorMessage = "Beskrivelse ma fylles ut")]
        [Display(Name = "Beskrivelse")]
        public string Beskrivelse { get; set; }

        public string GeometriGeoJson { get; set; }
    }
}
