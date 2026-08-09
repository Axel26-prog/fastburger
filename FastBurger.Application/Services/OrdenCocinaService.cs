using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Data;
using FastBurger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastBurger.Application.Services;

public class OrdenCocinaService : IOrdenCocinaService
{
    private readonly FastBurgerContext _context;
    private readonly ILogger<OrdenCocinaService> _logger;

    public OrdenCocinaService(FastBurgerContext context, ILogger<OrdenCocinaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CrearDesdePedidoAsync(int idPedido)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.DetallePedidos)
                .ThenInclude(d => d.IdProductoNavigation)
                    .ThenInclude(pr => pr.ProcesoPreparacion.PasoPreparacions)
            .Include(p => p.DetallePedidos)
                .ThenInclude(d => d.IdComboNavigation)
            .FirstOrDefaultAsync(p => p.IdPedido == idPedido);

        if (pedido == null)
            throw new InvalidOperationException($"No se encontró el pedido #{idPedido} para crear la orden de cocina.");

        var idEstacion = await ResolverEstacionAsync(pedido);

        var ordenCocina = new OrdenCocina
        {
            IdPedido = pedido.IdPedido,
            IdEstacion = idEstacion,
            Estado = "en_espera",
            Prioridad = 5,
            FechaIngreso = DateTime.Now
        };
        _context.OrdenCocinas.Add(ordenCocina);
        await _context.SaveChangesAsync();

        foreach (var detalle in pedido.DetallePedidos)
        {
            _context.OrdenCocinaItems.Add(new OrdenCocinaItem
            {
                IdOrdenCocina = ordenCocina.IdOrdenCocina,
                IdDetalle = detalle.IdDetalle,
                EstadoItem = "pendiente"
            });
        }

        await _context.SaveChangesAsync();
    }

    private async Task<int> ResolverEstacionAsync(Pedido pedido)
    {
        foreach (var detalle in pedido.DetallePedidos)
        {
            if (detalle.IdProducto.HasValue &&
                detalle.IdProductoNavigation?.ProcesoPreparacion?.PasoPreparacions != null &&
                detalle.IdProductoNavigation.ProcesoPreparacion.PasoPreparacions.Any())
            {
                var primerPaso = detalle.IdProductoNavigation.ProcesoPreparacion.PasoPreparacions
                    .OrderBy(pp => pp.Orden)
                    .FirstOrDefault();
                if (primerPaso != null)
                    return primerPaso.IdEstacion;
            }
        }

        var estacionGeneral = await _context.EstacionCocinas
            .FirstOrDefaultAsync(e => e.Activa && e.Nombre == "Cocina General");
        if (estacionGeneral != null)
            return estacionGeneral.IdEstacion;

        var primeraActiva = await _context.EstacionCocinas
            .FirstOrDefaultAsync(e => e.Activa);
        if (primeraActiva != null)
            return primeraActiva.IdEstacion;

        throw new InvalidOperationException("No existe ninguna estación de cocina activa. Semilla la estación 'Cocina General' antes de registrar pedidos.");
    }

    public async Task<IEnumerable<OrdenCocinaListaDTO>> GetPendientesAsync()
    {
        return await ProyectarOrdenesAsync(o => o.Estado == "en_espera");
    }

    public async Task<IEnumerable<OrdenCocinaListaDTO>> GetActivasAsync()
    {
        return await ProyectarOrdenesAsync(o => o.Estado == "en_espera" || o.Estado == "en_proceso");
    }

    public async Task<OrdenCocinaListaDTO?> GetByIdAsync(int idOrden)
    {
        var orden = await _context.OrdenCocinas
            .Include(o => o.IdPedidoNavigation.IdUsuarioNavigation)
            .Include(o => o.IdEstacionNavigation)
            .Include(o => o.OrdenCocinaItems)
                .ThenInclude(i => i.IdDetalleNavigation.IdProductoNavigation)
            .Include(o => o.OrdenCocinaItems)
                .ThenInclude(i => i.IdDetalleNavigation.IdComboNavigation)
            .FirstOrDefaultAsync(o => o.IdOrdenCocina == idOrden);

        if (orden == null) return null;
        return Mapear(orden);
    }

    public async Task<bool> IniciarPreparacionAsync(int idOrden)
    {
        var orden = await _context.OrdenCocinas
            .Include(o => o.IdPedidoNavigation)
            .FirstOrDefaultAsync(o => o.IdOrdenCocina == idOrden);

        if (orden == null) return false;

        if (orden.Estado != "en_espera")
            throw new InvalidOperationException($"La orden #{idOrden} no está en espera (estado actual: {orden.Estado}).");

        orden.Estado = "en_proceso";
        orden.FechaInicio = DateTime.Now;

        if (orden.IdPedidoNavigation != null && orden.IdPedidoNavigation.Estado == "aceptada")
            orden.IdPedidoNavigation.Estado = "preparacion";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarcarListaAsync(int idOrden)
    {
        var orden = await _context.OrdenCocinas
            .Include(o => o.IdPedidoNavigation)
            .Include(o => o.OrdenCocinaItems)
                .ThenInclude(i => i.IdDetalleNavigation)
            .FirstOrDefaultAsync(o => o.IdOrdenCocina == idOrden);

        if (orden == null) return false;

        if (orden.Estado != "en_proceso")
            throw new InvalidOperationException($"La orden #{idOrden} no está en preparación (estado actual: {orden.Estado}).");

        orden.Estado = "listo";
        orden.FechaFin = DateTime.Now;

        foreach (var item in orden.OrdenCocinaItems)
            item.EstadoItem = "listo";

        if (orden.IdPedidoNavigation != null)
        {
            orden.IdPedidoNavigation.Estado = "entregada";
            orden.IdPedidoNavigation.FechaEntregaReal = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<IEnumerable<OrdenCocinaListaDTO>> ProyectarOrdenesAsync(Func<OrdenCocina, bool> predicado)
    {
        var ordenes = await _context.OrdenCocinas
            .Include(o => o.IdPedidoNavigation.IdUsuarioNavigation)
            .Include(o => o.IdEstacionNavigation)
            .Include(o => o.OrdenCocinaItems)
                .ThenInclude(i => i.IdDetalleNavigation.IdProductoNavigation)
            .Include(o => o.OrdenCocinaItems)
                .ThenInclude(i => i.IdDetalleNavigation.IdComboNavigation)
            .Where(o => o.Estado == "en_espera" || o.Estado == "en_proceso")
            .OrderBy(o => o.Prioridad)
            .ThenBy(o => o.FechaIngreso)
            .ToListAsync();

        return ordenes.Where(predicado).Select(Mapear).ToList();
    }

    private static OrdenCocinaListaDTO Mapear(OrdenCocina orden)
    {
        return new OrdenCocinaListaDTO
        {
            IdOrdenCocina = orden.IdOrdenCocina,
            IdPedido = orden.IdPedido,
            NombreCliente = orden.IdPedidoNavigation?.IdUsuarioNavigation != null
                ? $"{orden.IdPedidoNavigation.IdUsuarioNavigation.Nombre} {orden.IdPedidoNavigation.IdUsuarioNavigation.Apellidos}"
                : null,
            NombreEstacion = orden.IdEstacionNavigation?.Nombre,
            Estado = orden.Estado,
            Prioridad = orden.Prioridad,
            FechaIngreso = orden.FechaIngreso,
            FechaInicio = orden.FechaInicio,
            Items = orden.OrdenCocinaItems.Select(i => new OrdenCocinaItemDTO
            {
                IdDetalle = i.IdDetalle,
                IdProducto = i.IdDetalleNavigation?.IdProducto,
                NombreProducto = i.IdDetalleNavigation?.IdProductoNavigation?.Nombre,
                IdCombo = i.IdDetalleNavigation?.IdCombo,
                NombreCombo = i.IdDetalleNavigation?.IdComboNavigation?.Nombre,
                Cantidad = i.IdDetalleNavigation?.Cantidad ?? 0,
                EstadoItem = i.EstadoItem,
                Notas = i.IdDetalleNavigation?.Notas
            }).ToList()
        };
    }
}