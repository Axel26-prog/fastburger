using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class ProcesoPreparacion
{
    public int IdProceso { get; set; }

    public int IdProducto { get; set; }

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual ICollection<PasoPreparacion> PasoPreparacions { get; set; } = new List<PasoPreparacion>();
}
