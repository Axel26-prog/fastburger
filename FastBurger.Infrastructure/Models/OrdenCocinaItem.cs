using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class OrdenCocinaItem
{
    public int IdOrdenCocina { get; set; }

    public int IdDetalle { get; set; }

    public string EstadoItem { get; set; } = null!;

    public virtual DetallePedido IdDetalleNavigation { get; set; } = null!;

    public virtual OrdenCocina IdOrdenCocinaNavigation { get; set; } = null!;
}
