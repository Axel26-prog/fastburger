using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class DireccionUsuario
{
    public int IdDireccion { get; set; }

    public int IdUsuario { get; set; }

    public string Alias { get; set; } = null!;

    public string Provincia { get; set; } = null!;

    public string Canton { get; set; } = null!;

    public string Distrito { get; set; } = null!;

    public string DireccionExacta { get; set; } = null!;

    public string? Referencia { get; set; }

    public bool Predeterminada { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
