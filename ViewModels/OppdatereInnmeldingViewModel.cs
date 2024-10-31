using System.Collections;
using System.Collections.Generic;
using Models.Models;
using Models.Entities;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ViewModels
{
    public class OppdatereInnmeldingViewModel
    {
        public List<InnmeldingModel> OppdatereInnmeldinger { get; set; }

        // Geometridata
        public Geometri GeometriData { get; set; }
    }
}
