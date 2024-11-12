using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{ //TODO:Denne er ikke en entitet, må flyttes til Models.Models
    public class SaksbehandlerNavnModel
    {
        public int Id { get; set; }
        public string Navn { get; set; }
    }
}
