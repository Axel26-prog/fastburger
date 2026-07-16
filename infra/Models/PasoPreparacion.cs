using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class PasoPreparacion
{
    public int IdPaso { get; set; }

    public int IdProceso { get; set; }

    public int IdEstacion { get; set; }

    public short Orden { get; set; }

    public string Descripcion { get; set; } = null!;

    public int TiempoMin { get; set; }

    public int? TemperaturaC { get; set; }

    public virtual EstacionCocina IdEstacionNavigation { get; set; } = null!;

    public virtual ProcesoPreparacion IdProcesoNavigation { get; set; } = null!;
}
