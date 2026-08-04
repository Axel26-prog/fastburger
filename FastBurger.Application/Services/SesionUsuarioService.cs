using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Data;
using FastBurger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace FastBurger.Application.Services;

public class SesionUsuarioService : ISesionUsuarioService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly FastBurgerContext _context;

    public SesionUsuarioService(IHttpContextAccessor httpContextAccessor, FastBurgerContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public int? ObtenerIdUsuarioActual()
    {
        return _httpContextAccessor.HttpContext?.Session.GetInt32("usuarioId");
    }

    public bool HaySesionActiva()
    {
        return ObtenerIdUsuarioActual().HasValue;
    }

    public async Task<Usuario> ObtenerUsuarioActualAsync()
    {
        var idUsuario = ObtenerIdUsuarioActual();
        if (!idUsuario.HasValue)
        {
            throw new InvalidOperationException(
                "No hay un usuario activo en la sesión. Vaya a /Sesion para seleccionar un usuario antes de continuar.");
        }

        var usuario = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario.Value);

        if (usuario == null)
        {
            throw new InvalidOperationException(
                $"El usuario con ID {idUsuario.Value} registrado en la sesión ya no existe. Vaya a /Sesion para seleccionar un usuario válido.");
        }

        return usuario;
    }
}
