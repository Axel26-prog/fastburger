using FastBurger.Application.DTOs;

namespace FastBurger.Application.Interfaces;

public interface IOrdenCocinaService
{
    Task CrearDesdePedidoAsync(int idPedido);
    Task<IEnumerable<OrdenCocinaListaDTO>> GetPendientesAsync();
    Task<IEnumerable<OrdenCocinaListaDTO>> GetActivasAsync();
    Task<OrdenCocinaListaDTO?> GetByIdAsync(int idOrden);
    Task<bool> IniciarPreparacionAsync(int idOrden);
    Task<bool> MarcarListaAsync(int idOrden);
}