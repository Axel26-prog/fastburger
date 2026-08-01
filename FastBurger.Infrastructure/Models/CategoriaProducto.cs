using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class CategoriaProducto
{
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? ImagenUrl { get; set; }

    public bool Activa { get; set; }

    public virtual ICollection<Combo> Combos { get; set; } = new List<Combo>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
