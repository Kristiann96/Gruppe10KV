using System.Collections;
using System.Collections.Generic;
using Models.Models;
using Models.Entities;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ViewModels
{
    public class OppdatereInnmeldingViewModel
    {
        public List<InnmeldingModel> OppdatereInnmeldinger { get; set; }

        // Innmeldingdata
        public int InnmeldingId { get; set; }
        public string Tittel { get; set; }
        public string Status { get; set; }
        public string Beskrivelse { get; set; }

        // Geometridata
        public Geometri Geometri { get; set; }
    }
}
