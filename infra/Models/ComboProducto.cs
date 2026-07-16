using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class ComboProducto
{
    public int IdCombo { get; set; }

    public int IdProducto { get; set; }

    public short Cantidad { get; set; }

    public virtual Combo IdComboNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
