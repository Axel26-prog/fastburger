using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

public class CarritoController : Controller
{
    private readonly ICarritoService _carritoService;
    private const int DEFAULT_USUARIO_ID = 1;

    public CarritoController(ICarritoService carritoService)
    {
        _carritoService = carritoService;
    }

    private int GetUsuarioIdActual()
    {
        if (int.TryParse(Request.Query["usuarioId"], out int usuarioId))
        {
            HttpContext.Session.SetInt32("usuarioId", usuarioId);
            return usuarioId;
        }
        if (HttpContext.Session.GetInt32("usuarioId").HasValue)
            return HttpContext.Session.GetInt32("usuarioId")!.Value;
        return DEFAULT_USUARIO_ID;
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarAlCarritoDTO dto)
    {
        dto.IdUsuario = GetUsuarioIdActual();

        try
        {
            var resultado = await _carritoService.AgregarProductoAsync(dto);
            return Json(new { success = true, item = resultado });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    public async Task<IActionResult> Index()
    {
        var usuarioId = GetUsuarioIdActual();
        var carrito = await _carritoService.GetCarritoActivoAsync(usuarioId);
        return View(carrito);
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarCantidad(int id, short cantidad)
    {
        var usuarioId = GetUsuarioIdActual();
        var resultado = await _carritoService.ActualizarCantidadAsync(id, cantidad, usuarioId);
        if (resultado)
        {
            var carrito = await _carritoService.GetCarritoActivoAsync(usuarioId);
            return Json(new { success = true, carrito });
        }
        return Json(new { success = false, message = "Error al actualizar cantidad" });
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        var usuarioId = GetUsuarioIdActual();
        var resultado = await _carritoService.EliminarItemAsync(id, usuarioId);
        return Json(new { success = resultado });
    }

    [HttpPost]
    public async Task<IActionResult> Vaciar()
    {
        var usuarioId = GetUsuarioIdActual();
        await _carritoService.VaciarCarritoAsync(usuarioId);
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> ObtenerContador()
    {
        var usuarioId = GetUsuarioIdActual();
        var carrito = await _carritoService.GetCarritoActivoAsync(usuarioId);
        var totalItems = carrito?.TotalItems ?? 0;
        return Json(new { totalItems });
    }

    public async Task<IActionResult> ObtenerCarrito()
    {
        var usuarioId = GetUsuarioIdActual();
        var carrito = await _carritoService.GetCarritoActivoAsync(usuarioId);
        if (carrito == null)
            return Json(new { success = false, message = "No hay carrito activo" });
        return Json(new { success = true, carrito });
    }
}
