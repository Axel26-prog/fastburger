using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

[Authorize(Roles = "Cocina")]
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

    public async Task<IActionResult> Index()
    {
        var usuario = await _sesionUsuarioService.ObtenerUsuarioActualAsync();
        var info = await _pedidoService.GetInfoUsuarioAsync(usuario.IdUsuario);
        ViewBag.UsuarioInfo = info;

        var ordenes = await _ordenCocinaService.GetActivasAsync();
        return View(ordenes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Iniciar(int id)
    {
        try
        {
            await _ordenCocinaService.IniciarPreparacionAsync(id);
            TempData["Success"] = "Orden #" + id + " en preparaci\u00f3n.";
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
        try
        {
            await _ordenCocinaService.MarcarListaAsync(id);
            TempData["Success"] = "Orden #" + id + " marcada como lista y pedido entregado.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}