using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class DetallePedido
{
    public int IdDetalle { get; set; }

    public int IdPedido { get; set; }

    public int? IdProducto { get; set; }

    public int? IdCombo { get; set; }

    public short Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public string? Notas { get; set; }

    public virtual Combo? IdComboNavigation { get; set; }

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;

    public virtual Producto? IdProductoNavigation { get; set; }

    public virtual ICollection<OrdenCocinaItem> OrdenCocinaItems { get; set; } = new List<OrdenCocinaItem>();
}
