﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    Copypublic class KnyttInnmeldingTilPersonViewModel
    {
        [Required(ErrorMessage = "E-post må fylles ut")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postformat")]
        public string Epost { get; set; }

        // Hidden fields fra tidligere - smlet opp fra KartfeilMarkering - KartfeilSkjema (+bekreftelse)
        public string GeometriGeoJson { get; set; }
        public string Tittel { get; set; }
        public string Beskrivelse { get; set; }
        public bool ErNodEtatKritisk { get; set; }
    }
}
