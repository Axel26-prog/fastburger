namespace FastBurger.Application.DTOs;

public class DashboardDTO
{
    public DateTime Fecha { get; set; }

    public List<TopProductoDTO> TopProductos { get; set; } = new();

    public List<PedidoPorEstadoDTO> PedidosPorEstado { get; set; } = new();

    public int TotalPedidosHoy => PedidosPorEstado.Sum(p => p.Cantidad);
}

public class TopProductoDTO
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public int CantidadVendida { get; set; }

    public string? ImagenUrl { get; set; }
}

public class PedidoPorEstadoDTO
{
    public string Estado { get; set; } = null!;

    public string EstadoTexto { get; set; } = null!;

    public int Cantidad { get; set; }
}