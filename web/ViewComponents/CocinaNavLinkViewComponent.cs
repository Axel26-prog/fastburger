using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.ViewComponents;

public class CocinaNavLinkViewComponent : ViewComponent
{
    private readonly IPedidoService _pedidoService;
    private readonly ISesionUsuarioService _sesionUsuarioService;

    public CocinaNavLinkViewComponent(IPedidoService pedidoService, ISesionUsuarioService sesionUsuarioService)
    {
        _pedidoService = pedidoService;
        _sesionUsuarioService = sesionUsuarioService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!_sesionUsuarioService.HaySesionActiva())
            return Content(string.Empty);

        var usuario = await _sesionUsuarioService.ObtenerUsuarioActualAsync();
        var info = await _pedidoService.GetInfoUsuarioAsync(usuario.IdUsuario);

        if (info == null || info.IdRol != 3)
            return Content(string.Empty);

        return View(true);
    }
}