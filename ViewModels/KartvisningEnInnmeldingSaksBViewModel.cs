﻿using Models.Models;
using Models.Entities;
using System;
using Models.DTOs;

namespace ViewModels
{
    public class KartvisningEnInnmeldingSaksBViewModel
    {
        // Innmelding detaljer
        public InnmeldingDetaljKartvisningSaksBModel InnmeldingDetaljer { get; set; }

        // Geometridata
        public Geometri GeometriData { get; set; }
    }
}