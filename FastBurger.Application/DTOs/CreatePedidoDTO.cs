namespace FastBurger.Application.DTOs;

public class CreatePedidoDTO
{
    public int IdUsuario { get; set; }
    public int IdEmpleado { get; set; }
    public int? IdDireccion { get; set; }
    public string TipoEntrega { get; set; } = "recoger";
    public decimal Descuento { get; set; }
    public decimal CostoEnvio { get; set; }
    public string? Notas { get; set; }
    public int IdMetodoPago { get; set; }
    public decimal MontoRecibido { get; set; }
    public string? ReferenciaPago { get; set; }
    public List<LineaDetalleDTO> LineasDetalle { get; set; } = new();
}

public class LineaDetalleDTO
{
    public int? IdProducto { get; set; }
    public int? IdCombo { get; set; }
    public short Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string? Notas { get; set; }
}

public class ResumenCarritoDTO
{
    public int IdCarrito { get; set; }
    public int IdUsuario { get; set; }
    public string Estado { get; set; } = null!;
    public int TotalItems { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Total { get; set; }
    public List<CarritoItemDTO> Items { get; set; } = new();
}

public class CarritoItemDTO
{
    public int IdItem { get; set; }
    public int? IdProducto { get; set; }
    public string? NombreProducto { get; set; }
    public string? ImagenProducto { get; set; }
    public int? IdCombo { get; set; }
    public string? NombreCombo { get; set; }
    public short Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string? Notas { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
    public decimal ImpuestoUnitario => Subtotal * 0.13m;
}

public class TotalesCalculoDTO
{
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Impuesto { get; set; }
    public decimal CostoEnvio { get; set; }
    public decimal Total { get; set; }
    public List<LineaTotalesDTO> Lineas { get; set; } = new();
}

public class LineaTotalesDTO
{
    public int? IdProducto { get; set; }
    public int? IdCombo { get; set; }
    public string? Nombre { get; set; }
    public short Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Total { get; set; }
}
