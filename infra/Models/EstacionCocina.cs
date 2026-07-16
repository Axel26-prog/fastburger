using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class EstacionCocina
{
    public int IdEstacion { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activa { get; set; }

    public virtual ICollection<OrdenCocina> OrdenCocinas { get; set; } = new List<OrdenCocina>();

    public virtual ICollection<PasoPreparacion> PasoPreparacions { get; set; } = new List<PasoPreparacion>();
}
