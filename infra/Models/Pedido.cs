using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class Pedido
{
    public int IdPedido { get; set; }

    public int IdUsuario { get; set; }

    public int? IdEmpleado { get; set; }

    public int IdCarrito { get; set; }

    public int? IdDireccion { get; set; }

    public string TipoEntrega { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuesto { get; set; }

    public decimal CostoEnvio { get; set; }

    public decimal Total { get; set; }

    public string? Notas { get; set; }

    public DateTime FechaPedido { get; set; }

    public DateTime? FechaEntregaEstimada { get; set; }

    public DateTime? FechaEntregaReal { get; set; }

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual Carrito IdCarritoNavigation { get; set; } = null!;

    public virtual DireccionUsuario? IdDireccionNavigation { get; set; }

    public virtual Usuario? IdEmpleadoNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<OrdenCocina> OrdenCocinas { get; set; } = new List<OrdenCocina>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
