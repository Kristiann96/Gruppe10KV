using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.SaksbehandlerModels;
public enum Statuser
{
    IkkeTattTilFølge,
    UnderBehandling,
    Avsluttet,
    IkkeLøst
}
public class InnmeldinSaksbehandlerModel
{
    public string Behandler { get; set; }

    public Statuser Status { get; set; }
}
