using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastBurger.Web.Controllers;

public class SesionController : Controller
{
    private readonly FastBurgerContext _context;
    private readonly ISesionUsuarioService _sesionUsuarioService;

    public SesionController(FastBurgerContext context, ISesionUsuarioService sesionUsuarioService)
    {
        _context = context;
        _sesionUsuarioService = sesionUsuarioService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Where(u => u.Activo)
            .OrderBy(u => u.IdRol)
            .ThenBy(u => u.Nombre)
            .Select(u => new InfoUsuarioDTO
            {
                IdUsuario = u.IdUsuario,
                Nombre = u.Nombre,
                Apellidos = u.Apellidos,
                Correo = u.Correo,
                Telefono = u.Telefono,
                IdRol = u.IdRol,
                NombreRol = u.IdRolNavigation.Nombre
            })
            .ToListAsync();

        ViewBag.Usuarios = usuarios;
        ViewBag.IdUsuarioActual = _sesionUsuarioService.ObtenerIdUsuarioActual();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Establecer(int idUsuario)
    {
        HttpContext.Session.SetInt32("usuarioId", idUsuario);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cerrar()
    {
        HttpContext.Session.Remove("usuarioId");
        return RedirectToAction(nameof(Index));
    }
}
