namespace FastBurger.Application.DTOs;

public class OrdenCocinaListaDTO
{
    public int IdOrdenCocina { get; set; }
    public int IdPedido { get; set; }
    public string? NombreCliente { get; set; }
    public string? NombreEstacion { get; set; }
    public string Estado { get; set; } = null!;
    public short Prioridad { get; set; }
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaInicio { get; set; }
    public int TiempoEsperaMin => (int)Math.Round((DateTime.Now - FechaIngreso).TotalMinutes);
    public int TiempoPreparacionMin => FechaInicio.HasValue ? (int)Math.Round((DateTime.Now - FechaInicio.Value).TotalMinutes) : 0;
    public List<OrdenCocinaItemDTO> Items { get; set; } = new();
}

public class OrdenCocinaItemDTO
{
    public int IdDetalle { get; set; }
    public int? IdProducto { get; set; }
    public string? NombreProducto { get; set; }
    public int? IdCombo { get; set; }
    public string? NombreCombo { get; set; }
    public short Cantidad { get; set; }
    public string EstadoItem { get; set; } = null!;
    public string? Notas { get; set; }
}