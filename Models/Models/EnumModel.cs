using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class EnumModel
    {
        public string Value { get; set; }
        public string DisplayName { get; set; }
        public EnumType Type { get; set; }
    }

    public enum EnumType
    {
        Status,
        Prioritet,
        KartType,
        InnmelderType
    }
}
