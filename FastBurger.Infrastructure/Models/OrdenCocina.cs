using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class OrdenCocina
{
    public int IdOrdenCocina { get; set; }

    public int IdPedido { get; set; }

    public int IdEstacion { get; set; }

    public string Estado { get; set; } = null!;

    public short Prioridad { get; set; }

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string? NotasCocina { get; set; }

    public virtual EstacionCocina IdEstacionNavigation { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;

    public virtual ICollection<OrdenCocinaItem> OrdenCocinaItems { get; set; } = new List<OrdenCocinaItem>();
}
