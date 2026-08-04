using FastBurger.Infrastructure.Models;

namespace FastBurger.Application.Interfaces;

public interface ISesionUsuarioService
{
    Task<Usuario> ObtenerUsuarioActualAsync();
    int? ObtenerIdUsuarioActual();
    bool HaySesionActiva();
}
