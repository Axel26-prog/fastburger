using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

public class PedidoController : Controller
{
    private readonly IPedidoService _pedidoService;
    private readonly ICarritoService _carritoService;
    private readonly ISesionUsuarioService _sesionUsuarioService;

    public PedidoController(IPedidoService pedidoService, ICarritoService carritoService, ISesionUsuarioService sesionUsuarioService)
    {
        _pedidoService = pedidoService;
        _carritoService = carritoService;
        _sesionUsuarioService = sesionUsuarioService;
    }

    private async Task<(int usuarioId, InfoUsuarioDTO? info)> GetUsuarioActualAsync()
    {
        if (!_sesionUsuarioService.HaySesionActiva())
        {
            return (0, null);
        }
        var usuario = await _sesionUsuarioService.ObtenerUsuarioActualAsync();
        var info = await _pedidoService.GetInfoUsuarioAsync(usuario.IdUsuario);
        return (usuario.IdUsuario, info);
    }

    public async Task<IActionResult> Index()
    {
        var (usuarioId, info) = await GetUsuarioActualAsync();
        if (info == null)
        {
            TempData["Error"] = "Debe seleccionar un usuario en /Sesion antes de continuar.";
            return RedirectToAction("Index", "Sesion");
        }
        ViewBag.UsuarioIdActual = usuarioId;
        ViewBag.UsuarioInfo = info;

        IEnumerable<PedidoDTO> pedidos;
        bool esAdmin = info.EsEncargadoOAdmin;

        if (esAdmin)
        {
            var filtro = new PedidoFiltroDTO();
            if (!string.IsNullOrEmpty(Request.Query["estado"]))
                filtro.Estado = Request.Query["estado"];
            if (int.TryParse(Request.Query["filtroUsuarioId"], out int filtroUsuarioId))
                filtro.IdUsuario = filtroUsuarioId;
            if (DateTime.TryParse(Request.Query["fechaDesde"], out DateTime fechaDesde))
                filtro.FechaDesde = fechaDesde;
            if (DateTime.TryParse(Request.Query["fechaHasta"], out DateTime fechaHasta))
                filtro.FechaHasta = fechaHasta;

            if (filtro.IdUsuario.HasValue || !string.IsNullOrEmpty(filtro.Estado) || filtro.FechaDesde.HasValue || filtro.FechaHasta.HasValue)
                pedidos = await _pedidoService.GetFilteredAsync(filtro);
            else
                pedidos = await _pedidoService.GetAllAsync();

            var clientes = await _pedidoService.GetClientesAsync();
            ViewBag.Clientes = clientes;
        }
        else
        {
            pedidos = await _pedidoService.GetByUsuarioAsync(usuarioId);
        }

        ViewBag.EsAdmin = esAdmin;
        return View(pedidos);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var (usuarioId, info) = await GetUsuarioActualAsync();
        if (info == null)
        {
            TempData["Error"] = "Debe seleccionar un usuario en /Sesion antes de continuar.";
            return RedirectToAction("Index", "Sesion");
        }
        var pedido = await _pedidoService.GetDetalleByIdAsync(id);
        if (pedido == null) return NotFound();

        bool esAdmin = info.EsEncargadoOAdmin;
        ViewBag.EsAdmin = esAdmin;

        if (!esAdmin && pedido.IdUsuario != usuarioId)
            return Forbid();

        ViewBag.UsuarioInfo = info;
        return View(pedido);
    }

    public async Task<IActionResult> Registrar()
    {
        var (usuarioId, info) = await GetUsuarioActualAsync();
        if (info == null)
        {
            TempData["Error"] = "Debe seleccionar un usuario en /Sesion antes de continuar.";
            return RedirectToAction("Index", "Sesion");
        }
        ViewBag.UsuarioIdActual = usuarioId;
        ViewBag.UsuarioInfo = info;

        bool esEncargadoOAdmin = info.EsEncargadoOAdmin;

        if (esEncargadoOAdmin)
        {
            var clientes = await _pedidoService.GetClientesAsync();
            ViewBag.Clientes = clientes;
        }

        var metodosPago = await _pedidoService.GetMetodosPagoAsync();
        ViewBag.MetodosPago = metodosPago;

        var carrito = await _carritoService.GetCarritoActivoAsync(usuarioId);
        ViewBag.Carrito = carrito;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerCliente(int id)
    {
        var info = await _pedidoService.GetInfoUsuarioAsync(id);
        if (info == null) return NotFound();

        var direcciones = await _pedidoService.GetDireccionesUsuarioAsync(id);
        return Json(new
        {
            info.IdUsuario,
            info.Nombre,
            info.Apellidos,
            info.Correo,
            info.Telefono,
            Direcciones = direcciones
        });
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerPrecio(int tipo, int id)
    {
        decimal precio = 0;
        string nombre = "";

        if (tipo == 1)
        {
            precio = await _pedidoService.GetPrecioProductoAsync(id);
            var producto = await _pedidoService.GetInfoUsuarioAsync(id);
        }
        else if (tipo == 2)
        {
            precio = await _pedidoService.GetPrecioComboAsync(id);
        }

        return Json(new { precio, nombre });
    }

    [HttpPost]
    public async Task<IActionResult> CalcularTotales([FromBody] CalcularTotalesRequest request)
    {
        if (request.Lineas == null)
            request.Lineas = new List<LineaDetalleDTO>();

        var totales = await _pedidoService.CalcularTotalesAsync(request.Lineas, request.TipoEntrega, request.Descuento);
        return Json(totales);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(CreatePedidoDTO dto)
    {
        var (usuarioId, info) = await GetUsuarioActualAsync();
        if (info == null)
        {
            TempData["Error"] = "Debe seleccionar un usuario en /Sesion antes de continuar.";
            return RedirectToAction("Index", "Sesion");
        }
        ViewBag.UsuarioIdActual = usuarioId;
        ViewBag.UsuarioInfo = info;

        bool esEncargadoOAdmin = info.EsEncargadoOAdmin;

        if (esEncargadoOAdmin)
        {
            var clientes = await _pedidoService.GetClientesAsync();
            ViewBag.Clientes = clientes;
        }

        var metodosPago = await _pedidoService.GetMetodosPagoAsync();
        ViewBag.MetodosPago = metodosPago;

        // El IdEmpleado y el IdUsuario se determinan SIEMPRE desde la sesión (servidor),
        // ignorando cualquier valor que venga del formulario/DTO.
        if (esEncargadoOAdmin)
        {
            // El encargado logueado es quien registra: su ID va como IdEmpleado.
            // El IdUsuario (cliente) se mantiene del DTO (lo eligió en el dropdown).
            dto.IdEmpleado = usuarioId;
        }
        else
        {
            // El cliente logueado es quien hace el pedido: su ID va como IdUsuario.
            // No hay encargado en este caso.
            dto.IdUsuario = usuarioId;
            dto.IdEmpleado = 0;
        }

        if (dto.LineasDetalle == null || !dto.LineasDetalle.Any())
        {
            ModelState.AddModelError(string.Empty, "Debe agregar al menos un producto o combo.");
            return View(dto);
        }

        foreach (var linea in dto.LineasDetalle)
        {
            if (linea.Cantidad <= 0)
            {
                ModelState.AddModelError(string.Empty, "Todas las cantidades deben ser mayores a cero.");
                return View(dto);
            }
        }

        try
        {
            var pedido = await _pedidoService.CreateAsync(dto);
            TempData["Success"] = $"Pedido #{pedido.IdPedido} registrado exitosamente.";
            return RedirectToAction(nameof(Detalle), new { id = pedido.IdPedido });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        return View(dto);
    }
}

public class CalcularTotalesRequest
{
    public List<LineaDetalleDTO> Lineas { get; set; } = new();
    public string TipoEntrega { get; set; } = "recoger";
    public decimal Descuento { get; set; }
}
