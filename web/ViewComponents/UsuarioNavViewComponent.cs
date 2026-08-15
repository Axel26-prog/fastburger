using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.ViewComponents;

public class UsuarioNavViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var user = HttpContext.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            return View(false);
        }

        var nombre = user.FindFirst("Nombre")?.Value ?? "";
        var apellidos = user.FindFirst("Apellidos")?.Value ?? "";
        var rol = user.FindFirst(ClaimTypes.Role)?.Value ?? "";

        ViewBag.NombreCompleto = $"{nombre} {apellidos}".Trim();
        ViewBag.Rol = rol;
        ViewBag.EsAdmin = string.Equals(rol, "Administrador", StringComparison.OrdinalIgnoreCase);

        return View(true);
    }
}