namespace FastBurger.Application.DTOs;

public class PedidoDTO
{
    public int IdPedido { get; set; }
    public int IdUsuario { get; set; }
    public string? NombreUsuario { get; set; }
    public string? CorreoUsuario { get; set; }
    public int? IdEmpleado { get; set; }
    public string? NombreEmpleado { get; set; }
    public int IdCarrito { get; set; }
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
    public int TotalItems { get; set; }
}

public class PedidoDetalleDTO
{
    public int IdPedido { get; set; }
    public int IdUsuario { get; set; }
    public string? NombreUsuario { get; set; }
    public string? CorreoUsuario { get; set; }
    public string? TelefonoUsuario { get; set; }
    public int? IdDireccion { get; set; }
    public string? DireccionEntrega { get; set; }
    public int? IdEmpleado { get; set; }
    public string? NombreEmpleado { get; set; }
    public int IdCarrito { get; set; }
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
    public List<DetallePedidoItemDTO> Items { get; set; } = new();
    public PagoDTO? Pago { get; set; }
}

public class DetallePedidoItemDTO
{
    public int IdDetalle { get; set; }
    public int? IdProducto { get; set; }
    public string? NombreProducto { get; set; }
    public int? IdCombo { get; set; }
    public string? NombreCombo { get; set; }
    public short Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal ImpuestoUnitario { get; set; }
    public string? Notas { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
    public decimal SubtotalConImpuesto => Subtotal + (ImpuestoUnitario * Cantidad);
}

public class PagoDTO
{
    public int IdPago { get; set; }
    public int IdMetodo { get; set; }
    public string? NombreMetodo { get; set; }
    public decimal Monto { get; set; }
    public decimal? MontoRecibido { get; set; }
    public string? Referencia { get; set; }
    public string Estado { get; set; } = null!;
    public DateTime FechaPago { get; set; }
}

public class PedidoFiltroDTO
{
    public int? IdUsuario { get; set; }
    public string? Estado { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
}

public class InfoUsuarioDTO
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string? Telefono { get; set; }
    public int IdRol { get; set; }
    public string? NombreRol { get; set; }
    public string NombreCompleto => $"{Nombre} {Apellidos}";
    public bool EsCliente => IdRol == 3;
    public bool EsEncargadoOAdmin => IdRol == 1 || IdRol == 2;
}
