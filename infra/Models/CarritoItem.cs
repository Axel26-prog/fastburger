using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class CarritoItem
{
    public int IdItem { get; set; }

    public int IdCarrito { get; set; }

    public int? IdProducto { get; set; }

    public int? IdCombo { get; set; }

    public short Cantidad { get; set; }

    public string? Notas { get; set; }

    public decimal PrecioUnitario { get; set; }

    public virtual Carrito IdCarritoNavigation { get; set; } = null!;

    public virtual Combo? IdComboNavigation { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
