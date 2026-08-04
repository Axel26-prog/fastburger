using FastBurger.Application.DTOs;

namespace FastBurger.Application.Interfaces;

public interface ICarritoService
{
    Task<ResumenCarritoDTO?> GetCarritoActivoAsync(int idUsuario);
    Task<CarritoItemAgregadoDTO> AgregarProductoAsync(AgregarAlCarritoDTO dto);
    Task<bool> EliminarItemAsync(int idItem, int idUsuario);
    Task<bool> VaciarCarritoAsync(int idUsuario);
    Task<ResumenCarritoDTO> ObtenerOCrearCarritoAsync(int idUsuario);
    Task<bool> ActualizarCantidadAsync(int idItem, short cantidad, int idUsuario);
}
