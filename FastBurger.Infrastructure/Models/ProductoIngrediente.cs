using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class ProductoIngrediente
{
    public int IdProducto { get; set; }

    public int IdIngrediente { get; set; }

    public decimal Cantidad { get; set; }

    public bool Opcional { get; set; }

    public virtual Ingrediente IdIngredienteNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
