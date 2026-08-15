using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.ViewComponents;

public class CarritoContadorViewComponent : ViewComponent
{
    private readonly IPedidoService _pedidoService;
    private readonly ISesionUsuarioService _sesionUsuarioService;

    public CarritoContadorViewComponent(IPedidoService pedidoService, ISesionUsuarioService sesionUsuarioService)
    {
        _pedidoService = pedidoService;
        _sesionUsuarioService = sesionUsuarioService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var usuarioId = _sesionUsuarioService.ObtenerIdUsuarioActual();

        int totalItems = 0;
        if (usuarioId.HasValue)
        {
            var carrito = await _pedidoService.GetCarritoActivoAsync(usuarioId.Value);
            totalItems = carrito?.TotalItems ?? 0;
        }

        ViewBag.UsuarioId = usuarioId ?? 0;
        return View(totalItems);
    }
}