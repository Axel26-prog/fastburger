using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.ViewComponents;

public class CarritoContadorViewComponent : ViewComponent
{
    private readonly IPedidoService _pedidoService;
    private const int DEFAULT_USUARIO_ID = 1;

    public CarritoContadorViewComponent(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    private int GetUsuarioIdActual()
    {
        if (int.TryParse(HttpContext.Request.Query["usuarioId"], out int usuarioId))
            return usuarioId;
        if (HttpContext.Session.GetInt32("usuarioId").HasValue)
            return HttpContext.Session.GetInt32("usuarioId")!.Value;
        return DEFAULT_USUARIO_ID;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var usuarioId = GetUsuarioIdActual();
        var carrito = await _pedidoService.GetCarritoActivoAsync(usuarioId);
        var totalItems = carrito?.TotalItems ?? 0;
        ViewBag.UsuarioId = usuarioId;
        return View(totalItems);
    }
}
