using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class MenuProducto
{
    public int IdMenu { get; set; }

    public int IdProducto { get; set; }

    public decimal? PrecioEspecial { get; set; }

    public virtual Menu IdMenuNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
