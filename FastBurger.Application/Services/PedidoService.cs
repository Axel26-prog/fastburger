using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Data;
using FastBurger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FastBurger.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly FastBurgerContext _context;
    private const decimal TASA_IMPUESTO = 0.13m;

    public PedidoService(FastBurgerContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PedidoDTO>> GetAllAsync()
    {
        return await _context.Pedidos
            .Include(p => p.IdUsuarioNavigation)
            .Include(p => p.IdEmpleadoNavigation)
            .Include(p => p.DetallePedidos)
            .OrderByDescending(p => p.FechaPedido)
            .Select(p => new PedidoDTO
            {
                IdPedido = p.IdPedido,
                IdUsuario = p.IdUsuario,
                NombreUsuario = p.IdUsuarioNavigation.Nombre + " " + p.IdUsuarioNavigation.Apellidos,
                CorreoUsuario = p.IdUsuarioNavigation.Correo,
                IdEmpleado = p.IdEmpleado,
                NombreEmpleado = p.IdEmpleado != null ? p.IdEmpleadoNavigation.Nombre + " " + p.IdEmpleadoNavigation.Apellidos : null,
                IdCarrito = p.IdCarrito,
                TipoEntrega = p.TipoEntrega,
                Estado = p.Estado,
                Subtotal = p.Subtotal,
                Descuento = p.Descuento,
                Impuesto = p.Impuesto,
                CostoEnvio = p.CostoEnvio,
                Total = p.Total,
                Notas = p.Notas,
                FechaPedido = p.FechaPedido,
                FechaEntregaEstimada = p.FechaEntregaEstimada,
                TotalItems = p.DetallePedidos.Sum(d => d.Cantidad)
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<PedidoDTO>> GetByUsuarioAsync(int idUsuario)
    {
        return await _context.Pedidos
            .Include(p => p.IdUsuarioNavigation)
            .Include(p => p.IdEmpleadoNavigation)
            .Include(p => p.DetallePedidos)
            .Where(p => p.IdUsuario == idUsuario)
            .OrderByDescending(p => p.FechaPedido)
            .Select(p => new PedidoDTO
            {
                IdPedido = p.IdPedido,
                IdUsuario = p.IdUsuario,
                NombreUsuario = p.IdUsuarioNavigation.Nombre + " " + p.IdUsuarioNavigation.Apellidos,
                CorreoUsuario = p.IdUsuarioNavigation.Correo,
                IdEmpleado = p.IdEmpleado,
                NombreEmpleado = p.IdEmpleado != null ? p.IdEmpleadoNavigation.Nombre + " " + p.IdEmpleadoNavigation.Apellidos : null,
                IdCarrito = p.IdCarrito,
                TipoEntrega = p.TipoEntrega,
                Estado = p.Estado,
                Subtotal = p.Subtotal,
                Descuento = p.Descuento,
                Impuesto = p.Impuesto,
                CostoEnvio = p.CostoEnvio,
                Total = p.Total,
                Notas = p.Notas,
                FechaPedido = p.FechaPedido,
                FechaEntregaEstimada = p.FechaEntregaEstimada,
                TotalItems = p.DetallePedidos.Sum(d => d.Cantidad)
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<PedidoDTO>> GetFilteredAsync(PedidoFiltroDTO filtro)
    {
        var query = _context.Pedidos
            .Include(p => p.IdUsuarioNavigation)
            .Include(p => p.IdEmpleadoNavigation)
            .Include(p => p.DetallePedidos)
            .AsQueryable();

        if (filtro.IdUsuario.HasValue)
            query = query.Where(p => p.IdUsuario == filtro.IdUsuario.Value);

        if (!string.IsNullOrEmpty(filtro.Estado))
            query = query.Where(p => p.Estado == filtro.Estado);

        if (filtro.FechaDesde.HasValue)
            query = query.Where(p => p.FechaPedido >= filtro.FechaDesde.Value);

        if (filtro.FechaHasta.HasValue)
            query = query.Where(p => p.FechaPedido <= filtro.FechaHasta.Value.AddDays(1));

        return await query
            .OrderByDescending(p => p.FechaPedido)
            .Select(p => new PedidoDTO
            {
                IdPedido = p.IdPedido,
                IdUsuario = p.IdUsuario,
                NombreUsuario = p.IdUsuarioNavigation.Nombre + " " + p.IdUsuarioNavigation.Apellidos,
                CorreoUsuario = p.IdUsuarioNavigation.Correo,
                IdEmpleado = p.IdEmpleado,
                NombreEmpleado = p.IdEmpleado != null ? p.IdEmpleadoNavigation.Nombre + " " + p.IdEmpleadoNavigation.Apellidos : null,
                IdCarrito = p.IdCarrito,
                TipoEntrega = p.TipoEntrega,
                Estado = p.Estado,
                Subtotal = p.Subtotal,
                Descuento = p.Descuento,
                Impuesto = p.Impuesto,
                CostoEnvio = p.CostoEnvio,
                Total = p.Total,
                Notas = p.Notas,
                FechaPedido = p.FechaPedido,
                FechaEntregaEstimada = p.FechaEntregaEstimada,
                TotalItems = p.DetallePedidos.Sum(d => d.Cantidad)
            })
            .ToListAsync();
    }

    public async Task<PedidoDetalleDTO?> GetDetalleByIdAsync(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.IdUsuarioNavigation)
            .Include(p => p.IdEmpleadoNavigation)
            .Include(p => p.IdDireccionNavigation)
            .Include(p => p.DetallePedidos)
                .ThenInclude(d => d.IdProductoNavigation)
            .Include(p => p.DetallePedidos)
                .ThenInclude(d => d.IdComboNavigation)
            .Include(p => p.Pagos)
                .ThenInclude(pg => pg.IdMetodoNavigation)
            .FirstOrDefaultAsync(p => p.IdPedido == id);

        if (pedido == null) return null;

        var direccionTexto = pedido.IdDireccionNavigation != null
            ? $"{pedido.IdDireccionNavigation.DireccionExacta}, {pedido.IdDireccionNavigation.Distrito}, {pedido.IdDireccionNavigation.Canton}, {pedido.IdDireccionNavigation.Provincia}"
            : null;

        return new PedidoDetalleDTO
        {
            IdPedido = pedido.IdPedido,
            IdUsuario = pedido.IdUsuario,
            NombreUsuario = pedido.IdUsuarioNavigation.Nombre + " " + pedido.IdUsuarioNavigation.Apellidos,
            CorreoUsuario = pedido.IdUsuarioNavigation.Correo,
            TelefonoUsuario = pedido.IdUsuarioNavigation.Telefono,
            IdDireccion = pedido.IdDireccion,
            DireccionEntrega = direccionTexto,
            IdEmpleado = pedido.IdEmpleado,
            NombreEmpleado = pedido.IdEmpleado != null ? pedido.IdEmpleadoNavigation.Nombre + " " + pedido.IdEmpleadoNavigation.Apellidos : null,
            IdCarrito = pedido.IdCarrito,
            TipoEntrega = pedido.TipoEntrega,
            Estado = pedido.Estado,
            Subtotal = pedido.Subtotal,
            Descuento = pedido.Descuento,
            Impuesto = pedido.Impuesto,
            CostoEnvio = pedido.CostoEnvio,
            Total = pedido.Total,
            Notas = pedido.Notas,
            FechaPedido = pedido.FechaPedido,
            FechaEntregaEstimada = pedido.FechaEntregaEstimada,
            FechaEntregaReal = pedido.FechaEntregaReal,
            Items = pedido.DetallePedidos.Select(d => new DetallePedidoItemDTO
            {
                IdDetalle = d.IdDetalle,
                IdProducto = d.IdProducto,
                NombreProducto = d.IdProductoNavigation?.Nombre,
                IdCombo = d.IdCombo,
                NombreCombo = d.IdComboNavigation?.Nombre,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                ImpuestoUnitario = d.PrecioUnitario * TASA_IMPUESTO,
                Notas = d.Notas
            }).ToList(),
            Pago = pedido.Pagos.FirstOrDefault() != null ? new PagoDTO
            {
                IdPago = pedido.Pagos.First().IdPago,
                IdMetodo = pedido.Pagos.First().IdMetodo,
                NombreMetodo = pedido.Pagos.First().IdMetodoNavigation.Nombre,
                Monto = pedido.Pagos.First().Monto,
                MontoRecibido = pedido.Pagos.First().MontoRecibido,
                Referencia = pedido.Pagos.First().Referencia,
                Estado = pedido.Pagos.First().Estado,
                FechaPago = pedido.Pagos.First().FechaPago
            } : null
        };
    }

    public async Task<ResumenCarritoDTO?> GetCarritoActivoAsync(int idUsuario)
    {
        var carrito = await _context.Carritos
            .Include(c => c.CarritoItems)
                .ThenInclude(ci => ci.IdProductoNavigation)
            .Include(c => c.CarritoItems)
                .ThenInclude(ci => ci.IdComboNavigation)
            .Where(c => c.IdUsuario == idUsuario && c.Estado == "activo")
            .FirstOrDefaultAsync();

        if (carrito == null) return null;

        var subtotal = carrito.CarritoItems.Sum(ci => ci.Cantidad * ci.PrecioUnitario);
        var impuesto = subtotal * TASA_IMPUESTO;

        return new ResumenCarritoDTO
        {
            IdCarrito = carrito.IdCarrito,
            IdUsuario = carrito.IdUsuario,
            Estado = carrito.Estado,
            TotalItems = carrito.CarritoItems.Sum(ci => ci.Cantidad),
            Subtotal = subtotal,
            Impuesto = impuesto,
            Total = subtotal + impuesto,
            Items = carrito.CarritoItems.Select(ci => new CarritoItemDTO
            {
                IdItem = ci.IdItem,
                IdProducto = ci.IdProducto,
                NombreProducto = ci.IdProductoNavigation?.Nombre,
                ImagenProducto = ci.IdProductoNavigation?.ImagenUrl,
                IdCombo = ci.IdCombo,
                NombreCombo = ci.IdComboNavigation?.Nombre,
                Cantidad = ci.Cantidad,
                PrecioUnitario = ci.PrecioUnitario,
                Notas = ci.Notas
            }).ToList()
        };
    }

    public async Task<PedidoDetalleDTO> CreateAsync(CreatePedidoDTO dto)
    {
        if (dto.LineasDetalle == null || !dto.LineasDetalle.Any())
            throw new InvalidOperationException("Debe agregar al menos un producto o combo al pedido.");

        var subtotal = dto.LineasDetalle.Sum(l => l.Cantidad * l.PrecioUnitario);
        var descuento = dto.Descuento;
        var impuesto = (subtotal - descuento) * TASA_IMPUESTO;
        var total = subtotal - descuento + impuesto + dto.CostoEnvio;

        var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado.");

        var metodoPago = await _context.MetodoPagos.FindAsync(dto.IdMetodoPago);
        if (metodoPago == null)
            throw new InvalidOperationException("Método de pago no encontrado.");

        var carrito = new Carrito
        {
            IdUsuario = dto.IdUsuario,
            Estado = "procesado",
            FechaCreacion = DateTime.Now,
            FechaActualizacion = DateTime.Now
        };
        _context.Carritos.Add(carrito);
        await _context.SaveChangesAsync();

        var pedido = new Pedido
        {
            IdUsuario = dto.IdUsuario,
            IdEmpleado = dto.IdEmpleado > 0 ? dto.IdEmpleado : null,
            IdCarrito = carrito.IdCarrito,
            IdDireccion = dto.IdDireccion,
            TipoEntrega = dto.TipoEntrega,
            Estado = "aceptada",
            Subtotal = subtotal,
            Descuento = descuento,
            Impuesto = impuesto,
            CostoEnvio = dto.CostoEnvio,
            Total = total,
            Notas = dto.Notas,
            FechaPedido = DateTime.Now,
            FechaEntregaEstimada = DateTime.Now.AddMinutes(30)
        };

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        foreach (var linea in dto.LineasDetalle)
        {
            var detalle = new DetallePedido
            {
                IdPedido = pedido.IdPedido,
                IdProducto = linea.IdProducto,
                IdCombo = linea.IdCombo,
                Cantidad = linea.Cantidad,
                PrecioUnitario = linea.PrecioUnitario,
                Notas = linea.Notas
            };
            _context.DetallePedidos.Add(detalle);
        }

        var pago = new Pago
        {
            IdPedido = pedido.IdPedido,
            IdMetodo = dto.IdMetodoPago,
            Monto = total,
            MontoRecibido = dto.MontoRecibido >= total ? dto.MontoRecibido : total,
            Referencia = dto.ReferenciaPago ?? $"PAGO-{DateTime.Now:yyyyMMddHHmmss}",
            Estado = "aprobado",
            FechaPago = DateTime.Now
        };

        if (metodoPago.Nombre.Contains("Efectivo", StringComparison.OrdinalIgnoreCase))
        {
            if (dto.MontoRecibido < total)
                throw new InvalidOperationException($"El monto recibido (₡{dto.MontoRecibido:N2}) es insuficiente. El total a pagar es ₡{total:N2}.");
            pago.MontoRecibido = dto.MontoRecibido;
        }
        else
        {
            pago.MontoRecibido = dto.MontoRecibido >= total ? dto.MontoRecibido : total;
        }

        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync();

        return await GetDetalleByIdAsync(pedido.IdPedido)!;
    }

    public async Task<IEnumerable<MetodoPagoDTO>> GetMetodosPagoAsync()
    {
        return await _context.MetodoPagos
            .Where(m => m.Activo)
            .Select(m => new MetodoPagoDTO
            {
                IdMetodo = m.IdMetodo,
                Nombre = m.Nombre
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<UsuarioDTO>> GetUsuariosAsync()
    {
        return await _context.Usuarios
            .Where(u => u.Activo)
            .Select(u => new UsuarioDTO
            {
                IdUsuario = u.IdUsuario,
                Nombre = u.Nombre,
                Apellidos = u.Apellidos,
                Correo = u.Correo
            })
            .ToListAsync();
    }

    public async Task<InfoUsuarioDTO?> GetInfoUsuarioAsync(int idUsuario)
    {
        return await _context.Usuarios
            .Where(u => u.IdUsuario == idUsuario)
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
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<InfoUsuarioDTO>> GetClientesAsync()
    {
        return await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Where(u => u.Activo && u.IdRol == 3)
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
    }

    public async Task<TotalesCalculoDTO> CalcularTotalesAsync(List<LineaDetalleDTO> lineas, string tipoEntrega, decimal descuento)
    {
        var costoEnvio = tipoEntrega == "domicilio" ? 500m : 0m;
        var resultado = new TotalesCalculoDTO
        {
            Descuento = descuento,
            CostoEnvio = costoEnvio,
            Lineas = new List<LineaTotalesDTO>()
        };

        foreach (var linea in lineas)
        {
            var nombre = "";
            if (linea.IdProducto.HasValue)
            {
                var producto = await _context.Productos.FindAsync(linea.IdProducto.Value);
                nombre = producto?.Nombre ?? "";
            }
            else if (linea.IdCombo.HasValue)
            {
                var combo = await _context.Combos.FindAsync(linea.IdCombo.Value);
                nombre = combo?.Nombre ?? "";
            }

            var subtotalLinea = linea.Cantidad * linea.PrecioUnitario;
            var impuestoLinea = subtotalLinea * TASA_IMPUESTO;

            resultado.Lineas.Add(new LineaTotalesDTO
            {
                IdProducto = linea.IdProducto,
                IdCombo = linea.IdCombo,
                Nombre = nombre,
                Cantidad = linea.Cantidad,
                PrecioUnitario = linea.PrecioUnitario,
                Subtotal = subtotalLinea,
                Impuesto = impuestoLinea,
                Total = subtotalLinea + impuestoLinea
            });
        }

        resultado.Subtotal = resultado.Lineas.Sum(l => l.Subtotal);
        resultado.Impuesto = resultado.Lineas.Sum(l => l.Impuesto);
        resultado.Total = resultado.Subtotal - resultado.Descuento + resultado.Impuesto + resultado.CostoEnvio;

        return resultado;
    }

    public async Task<decimal> GetPrecioProductoAsync(int idProducto)
    {
        var producto = await _context.Productos.FindAsync(idProducto);
        return producto?.Precio ?? 0;
    }

    public async Task<decimal> GetPrecioComboAsync(int idCombo)
    {
        var combo = await _context.Combos.FindAsync(idCombo);
        return combo?.Precio ?? 0;
    }

    public async Task<IEnumerable<DireccionUsuarioDTO>> GetDireccionesUsuarioAsync(int idUsuario)
    {
        return await _context.DireccionUsuarios
            .Where(d => d.IdUsuario == idUsuario)
            .Select(d => new DireccionUsuarioDTO
            {
                IdDireccion = d.IdDireccion,
                Alias = d.Alias,
                DireccionExacta = d.DireccionExacta,
                Distrito = d.Distrito,
                Canton = d.Canton,
                Provincia = d.Provincia,
                Referencia = d.Referencia
            })
            .ToListAsync();
    }
}
