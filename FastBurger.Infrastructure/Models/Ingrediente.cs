using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class Ingrediente
{
    public int IdIngrediente { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Alergenico { get; set; }

    public string UnidadMedida { get; set; } = null!;

    public virtual ICollection<ProductoIngrediente> ProductoIngredientes { get; set; } = new List<ProductoIngrediente>();
}
