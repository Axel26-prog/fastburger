using FastBurger.Application.DTOs;

namespace FastBurger.Application.Interfaces;

public interface IPedidoService
{
    Task<IEnumerable<PedidoDTO>> GetAllAsync();
    Task<IEnumerable<PedidoDTO>> GetByUsuarioAsync(int idUsuario);
    Task<IEnumerable<PedidoDTO>> GetFilteredAsync(PedidoFiltroDTO filtro);
    Task<PedidoDetalleDTO?> GetDetalleByIdAsync(int id);
    Task<ResumenCarritoDTO?> GetCarritoActivoAsync(int idUsuario);
    Task<PedidoDetalleDTO> CreateAsync(CreatePedidoDTO dto);
    Task<IEnumerable<MetodoPagoDTO>> GetMetodosPagoAsync();
    Task<IEnumerable<UsuarioDTO>> GetUsuariosAsync();
    Task<InfoUsuarioDTO?> GetInfoUsuarioAsync(int idUsuario);
    Task<IEnumerable<InfoUsuarioDTO>> GetClientesAsync();
    Task<TotalesCalculoDTO> CalcularTotalesAsync(List<LineaDetalleDTO> lineas, string tipoEntrega, decimal descuento);
    Task<decimal> GetPrecioProductoAsync(int idProducto);
    Task<decimal> GetPrecioComboAsync(int idCombo);
    Task<IEnumerable<DireccionUsuarioDTO>> GetDireccionesUsuarioAsync(int idUsuario);
}

public class MetodoPagoDTO
{
    public int IdMetodo { get; set; }
    public string Nombre { get; set; } = null!;
}

public class UsuarioDTO
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string NombreCompleto => $"{Nombre} {Apellidos}";
}

public class DireccionUsuarioDTO
{
    public int IdDireccion { get; set; }
    public string? Alias { get; set; }
    public string? DireccionExacta { get; set; }
    public string? Distrito { get; set; }
    public string? Canton { get; set; }
    public string? Provincia { get; set; }
    public string? Referencia { get; set; }
    public string Completa => $"{DireccionExacta}, {Distrito}, {Canton}, {Provincia}";
}
