using System.Security.Claims;
using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Data;
using FastBurger.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var id) ? id : null;
    }

    public bool HaySesionActiva()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }

    public async Task<Usuario> ObtenerUsuarioActualAsync()
    {
        var idUsuario = ObtenerIdUsuarioActual();
        if (!idUsuario.HasValue)
        {
            throw new InvalidOperationException(
                "No hay un usuario con sesi\u00f3n activa. Inicie sesi\u00f3n para continuar.");
        }

        var usuario = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario.Value);

        if (usuario == null)
        {
            throw new InvalidOperationException(
                $"El usuario con ID {idUsuario.Value} registrado en la sesi\u00f3n ya no existe. Inicie sesi\u00f3n con un usuario v\u00e1lido.");
        }

        return usuario;
    }
}