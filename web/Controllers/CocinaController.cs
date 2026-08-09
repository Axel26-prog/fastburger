using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

public class CocinaController : Controller
{
    private readonly IOrdenCocinaService _ordenCocinaService;
    private readonly IPedidoService _pedidoService;
    private readonly ISesionUsuarioService _sesionUsuarioService;

    public CocinaController(IOrdenCocinaService ordenCocinaService, IPedidoService pedidoService, ISesionUsuarioService sesionUsuarioService)
    {
        _ordenCocinaService = ordenCocinaService;
        _pedidoService = pedidoService;
        _sesionUsuarioService = sesionUsuarioService;
    }

    private async Task<InfoUsuarioDTO?> GetUsuarioActualAsync()
    {
        if (!_sesionUsuarioService.HaySesionActiva())
            return null;
        var usuario = await _sesionUsuarioService.ObtenerUsuarioActualAsync();
        return await _pedidoService.GetInfoUsuarioAsync(usuario.IdUsuario);
    }

    public async Task<IActionResult> Index()
    {
        var info = await GetUsuarioActualAsync();
        if (info == null)
        {
            TempData["Error"] = "Debe seleccionar un usuario en /Sesion antes de continuar.";
            return RedirectToAction("Index", "Sesion");
        }

        if (info.IdRol != 3)
        {
            TempData["Error"] = "Acceso restringido. Debe iniciar sesión con un usuario con rol Cocina.";
            return RedirectToAction("Index", "Sesion");
        }

        ViewBag.UsuarioInfo = info;

        var ordenes = await _ordenCocinaService.GetActivasAsync();
        return View(ordenes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Iniciar(int id)
    {
        var info = await GetUsuarioActualAsync();
        if (info == null || info.IdRol != 3)
            return RedirectToAction("Index", "Sesion");

        try
        {
            await _ordenCocinaService.IniciarPreparacionAsync(id);
            TempData["Success"] = $"Orden #{id} en preparación.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarLista(int id)
    {
        var info = await GetUsuarioActualAsync();
        if (info == null || info.IdRol != 3)
            return RedirectToAction("Index", "Sesion");

        try
        {
            await _ordenCocinaService.MarcarListaAsync(id);
            TempData["Success"] = $"Orden #{id} marcada como lista y pedido entregado.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}