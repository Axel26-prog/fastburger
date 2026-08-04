namespace FastBurger.Application.DTOs;

public class AgregarAlCarritoDTO
{
    public int IdUsuario { get; set; }
    public int? IdProducto { get; set; }
    public int? IdCombo { get; set; }
    public short Cantidad { get; set; } = 1;
    public string? Notas { get; set; }
}

public class CarritoItemAgregadoDTO
{
    public int IdItem { get; set; }
    public int IdCarrito { get; set; }
    public int? IdProducto { get; set; }
    public string? NombreProducto { get; set; }
    public int? IdCombo { get; set; }
    public string? NombreCombo { get; set; }
    public short Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
