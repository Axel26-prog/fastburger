using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    public string Contrasena { get; set; } = null!;

    public string? FotoPerfil { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public string? TokenReset { get; set; }

    public DateTime? TokenResetExpira { get; set; }

    public virtual ICollection<Carrito> Carritos { get; set; } = new List<Carrito>();

    public virtual ICollection<DireccionUsuario> DireccionUsuarios { get; set; } = new List<DireccionUsuario>();

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Pedido> PedidoIdEmpleadoNavigations { get; set; } = new List<Pedido>();

    public virtual ICollection<Pedido> PedidoIdUsuarioNavigations { get; set; } = new List<Pedido>();
}
