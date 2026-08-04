using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

public class PedidoController : Controller
{
    private readonly IPedidoService _pedidoService;
    private readonly ICarritoService _carritoService;
    private const int DEFAULT_USUARIO_ID = 1;
    private const int ROL_CLIENTE = 3;
    private const int ROL_ENCARGADO = 2;
    private const int ROL_ADMIN = 1;

    public PedidoController(IPedidoService pedidoService, ICarritoService carritoService)
    {
        _pedidoService = pedidoService;
        _carritoService = carritoService;
    }

    private int GetUsuarioIdActual()
    {
        if (HttpContext.Session.GetInt32("usuarioId").HasValue)
            return HttpContext.Session.GetInt32("usuarioId")!.Value;
        return DEFAULT_USUARIO_ID;
    }

    private async Task<(int usuarioId, InfoUsuarioDTO? info)> GetUsuarioActualAsync()
    {
        var usuarioId = GetUsuarioIdActual();
        var info = await _pedidoService.GetInfoUsuarioAsync(usuarioId);
        return (usuarioId, info);
    }

    public async Task<IActionResult> Index()
    {
        var (usuarioId, info) = await GetUsuarioActualAsync();
        ViewBag.UsuarioIdActual = usuarioId;
        ViewBag.UsuarioInfo = info;

        IEnumerable<PedidoDTO> pedidos;
        bool esAdmin = info?.EsEncargadoOAdmin == true;

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
        var pedido = await _pedidoService.GetDetalleByIdAsync(id);
        if (pedido == null) return NotFound();

        bool esAdmin = info?.EsEncargadoOAdmin == true;
        ViewBag.EsAdmin = esAdmin;

        if (!esAdmin && pedido.IdUsuario != usuarioId)
            return Forbid();

        ViewBag.UsuarioInfo = info;
        return View(pedido);
    }

    public async Task<IActionResult> Registrar()
    {
        var (usuarioId, info) = await GetUsuarioActualAsync();
        ViewBag.UsuarioIdActual = usuarioId;
        ViewBag.UsuarioInfo = info;

        bool esEncargadoOAdmin = info?.EsEncargadoOAdmin == true;

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
        ViewBag.UsuarioIdActual = usuarioId;
        ViewBag.UsuarioInfo = info;

        bool esEncargadoOAdmin = info?.EsEncargadoOAdmin == true;

        if (esEncargadoOAdmin)
        {
            var clientes = await _pedidoService.GetClientesAsync();
            ViewBag.Clientes = clientes;
        }

        var metodosPago = await _pedidoService.GetMetodosPagoAsync();
        ViewBag.MetodosPago = metodosPago;

        if (!esEncargadoOAdmin)
        {
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
