using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastBurger.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly FastBurgerContext _context;

    public DashboardService(FastBurgerContext context)
    {
        _context = context;
    }

    public async Task<DashboardDTO> ObtenerResumenHoyAsync()
    {
        var hoy = DateTime.Today;
        var manana = hoy.AddDays(1);

        var topProductos = await _context.DetallePedidos
            .Where(d => d.IdProducto.HasValue
                     && d.IdPedidoNavigation.FechaPedido >= hoy
                     && d.IdPedidoNavigation.FechaPedido < manana)
            .GroupBy(d => new { d.IdProducto, d.IdProductoNavigation!.Nombre, d.IdProductoNavigation.ImagenUrl })
            .Select(g => new TopProductoDTO
            {
                IdProducto = g.Key.IdProducto!.Value,
                Nombre = g.Key.Nombre,
                ImagenUrl = g.Key.ImagenUrl,
                CantidadVendida = g.Sum(x => (int)x.Cantidad)
            })
            .OrderByDescending(x => x.CantidadVendida)
            .Take(3)
            .ToListAsync();

        var conteoEstados = await _context.Pedidos
            .Where(p => p.FechaPedido >= hoy && p.FechaPedido < manana)
            .GroupBy(p => p.Estado)
            .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
            .ToDictionaryAsync(x => x.Estado, x => x.Cantidad);
       
        var estadosOrden = new (string Clave, string Texto)[]
        {
            ("pendiente_pago", "Pendiente de pago"),
            ("aceptada", "Aceptada"),
            ("preparacion", "Preparación"),
            ("procesando", "Procesando"),
            ("entregada", "Entregada"),
            ("cancelada", "Cancelada")
        };

        var porEstado = estadosOrden
            .Select(e => new PedidoPorEstadoDTO
            {
                Estado = e.Clave,
                EstadoTexto = e.Texto,
                Cantidad = conteoEstados.TryGetValue(e.Clave, out var c) ? c : 0
            })
            .ToList();

        return new DashboardDTO
        {
            Fecha = hoy,
            TopProductos = topProductos,
            PedidosPorEstado = porEstado
        };
    }
}