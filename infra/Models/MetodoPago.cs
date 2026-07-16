using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class MetodoPago
{
    public int IdMetodo { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
