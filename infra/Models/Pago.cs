using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int IdPedido { get; set; }

    public int IdMetodo { get; set; }

    public decimal Monto { get; set; }

    public decimal? MontoRecibido { get; set; }

    public string? Referencia { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaPago { get; set; }

    public virtual MetodoPago IdMetodoNavigation { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}
