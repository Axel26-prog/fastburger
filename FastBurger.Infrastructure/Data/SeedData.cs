using FastBurger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FastBurger.Infrastructure.Data;

public static class SeedData
{
    private const decimal TASA_IMPUESTO = 0.13m;
    private const decimal COSTO_ENVIO = 500m;

    public static async Task InitializeAsync(FastBurgerContext context)
    {
        await SembrarEstacionesAsync(context);

        if (await context.Pedidos.CountAsync() >= 4)
            return;

        await SembrarDireccionesAsync(context);

        var dirUsuario1 = await context.DireccionUsuarios.FirstOrDefaultAsync(d => d.IdUsuario == 1);
        var dirUsuario2 = await context.DireccionUsuarios.FirstOrDefaultAsync(d => d.IdUsuario == 2);
        var dirUsuario3 = await context.DireccionUsuarios.FirstOrDefaultAsync(d => d.IdUsuario == 3);

        var specs = ConstruirSpecsPedidos(dirUsuario1?.IdDireccion, dirUsuario2?.IdDireccion, dirUsuario3?.IdDireccion);

        var carritos = specs
            .Select(s => new Carrito
            {
                IdUsuario = s.IdUsuario,
                Estado = "procesado",
                FechaCreacion = s.FechaPedido,
                FechaActualizacion = s.FechaPedido
            })
            .ToList();
        await context.Carritos.AddRangeAsync(carritos);
        await context.SaveChangesAsync();

        for (int i = 0; i < specs.Count; i++)
        {
            var spec = specs[i];
            var carrito = carritos[i];

            var subtotal = spec.Items.Sum(it => it.Cantidad * it.PrecioUnitario);
            var impuesto = Math.Round(subtotal * TASA_IMPUESTO, 2);
            var costoEnvio = spec.TipoEntrega == "domicilio" ? COSTO_ENVIO : 0m;
            var total = subtotal + impuesto + costoEnvio;

            var pedido = new Pedido
            {
                IdUsuario = spec.IdUsuario,
                IdCarrito = carrito.IdCarrito,
                IdDireccion = spec.IdDireccion,
                TipoEntrega = spec.TipoEntrega,
                Estado = spec.Estado,
                Subtotal = subtotal,
                Descuento = 0m,
                Impuesto = impuesto,
                CostoEnvio = costoEnvio,
                Total = total,
                Notas = spec.Notas,
                FechaPedido = spec.FechaPedido,
                FechaEntregaEstimada = spec.FechaPedido.AddMinutes(30),
                FechaEntregaReal = spec.Estado == "entregada"
                    ? spec.FechaPedido.AddMinutes(25)
                    : null
            };

            foreach (var item in spec.Items)
            {
                pedido.DetallePedidos.Add(new DetallePedido
                {
                    IdProducto = item.IdProducto,
                    IdCombo = item.IdCombo,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario,
                    Notas = item.Notas
                });
            }

            pedido.Pagos.Add(new Pago
            {
                IdMetodo = spec.IdMetodoPago,
                Monto = total,
                MontoRecibido = spec.MontoRecibido,
                Referencia = spec.ReferenciaPago,
                Estado = spec.PagoEstado,
                FechaPago = spec.FechaPedido
            });

            context.Pedidos.Add(pedido);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SembrarEstacionesAsync(FastBurgerContext context)
    {
        if (!await context.EstacionCocinas.AnyAsync(e => e.Nombre == "Cocina General"))
        {
            await context.EstacionCocinas.AddAsync(new EstacionCocina
            {
                Nombre = "Cocina General",
                Descripcion = "Estación por defecto para órdenes sin estación específica asignada.",
                Activa = true
            });
            await context.SaveChangesAsync();
        }
    }

    private static async Task SembrarDireccionesAsync(FastBurgerContext context)
    {
        if (!await context.DireccionUsuarios.AnyAsync())
        {
            await context.DireccionUsuarios.AddRangeAsync(
                new DireccionUsuario
                {
                    IdUsuario = 1,
                    Alias = "Casa",
                    Provincia = "San José",
                    Canton = "San José",
                    Distrito = "Merced",
                    DireccionExacta = "Avenida 0, Calle 5",
                    Predeterminada = true
                },
                new DireccionUsuario
                {
                    IdUsuario = 2,
                    Alias = "Casa",
                    Provincia = "San José",
                    Canton = "Escazú",
                    Distrito = "San Rafael",
                    DireccionExacta = "Condominio Las Acacias, Casa 10",
                    Predeterminada = true
                },
                new DireccionUsuario
                {
                    IdUsuario = 3,
                    Alias = "Trabajo",
                    Provincia = "San José",
                    Canton = "Montes de Oca",
                    Distrito = "San Pedro",
                    DireccionExacta = "Avenida 2, Calle 25",
                    Predeterminada = true
                }
            );
            await context.SaveChangesAsync();
        }
    }

    private static List<SpecPedido> ConstruirSpecsPedidos(int? dirUsuario1, int? dirUsuario2, int? dirUsuario3)
    {
        var ahora = DateTime.Now;

        return new List<SpecPedido>
        {
            new SpecPedido
            {
                IdUsuario = 1,
                TipoEntrega = "recoger",
                Estado = "entregada",
                IdDireccion = null,
                FechaPedido = ahora.AddDays(-7),
                Notas = "Sin cebolla en las burgers",
                IdMetodoPago = 1,
                PagoEstado = "aprobado",
                MontoRecibido = 20000m,
                ReferenciaPago = "SEED-EFE-1",
                Items =
                {
                    (IdProducto: 4, IdCombo: (int?)null, Cantidad: (short)2, PrecioUnitario: 5500m, Notas: (string?)null),
                    (IdProducto: (int?)null, IdCombo: 1, Cantidad: (short)1, PrecioUnitario: 5500m, Notas: (string?)null)
                }
            },
            new SpecPedido
            {
                IdUsuario = 2,
                TipoEntrega = "domicilio",
                Estado = "entregada",
                IdDireccion = dirUsuario2,
                FechaPedido = ahora.AddDays(-5),
                Notas = "Tocar timbre dos veces",
                IdMetodoPago = 2,
                PagoEstado = "aprobado",
                MontoRecibido = null,
                ReferenciaPago = "SEED-TDC-2",
                Items =
                {
                    (IdProducto: (int?)null, IdCombo: 2, Cantidad: (short)1, PrecioUnitario: 6500m, Notas: (string?)null),
                    (IdProducto: 7, IdCombo: (int?)null, Cantidad: (short)2, PrecioUnitario: 1000m, Notas: (string?)null)
                }
            },
            new SpecPedido
            {
                IdUsuario = 1,
                TipoEntrega = "recoger",
                Estado = "preparacion",
                IdDireccion = null,
                FechaPedido = ahora.AddDays(-3),
                Notas = null,
                IdMetodoPago = 3,
                PagoEstado = "aprobado",
                MontoRecibido = null,
                ReferenciaPago = "SEED-TDD-3",
                Items =
                {
                    (IdProducto: 1, IdCombo: (int?)null, Cantidad: (short)1, PrecioUnitario: 3500m, Notas: (string?)null),
                    (IdProducto: 5, IdCombo: (int?)null, Cantidad: (short)1, PrecioUnitario: 1500m, Notas: (string?)null)
                }
            },
            new SpecPedido
            {
                IdUsuario = 3,
                TipoEntrega = "domicilio",
                Estado = "procesando",
                IdDireccion = dirUsuario3,
                FechaPedido = ahora.AddDays(-2),
                Notas = null,
                IdMetodoPago = 1,
                PagoEstado = "aprobado",
                MontoRecibido = 12000m,
                ReferenciaPago = "SEED-EFE-4",
                Items =
                {
                    (IdProducto: (int?)null, IdCombo: 4, Cantidad: (short)1, PrecioUnitario: 8500m, Notas: (string?)null),
                    (IdProducto: 8, IdCombo: (int?)null, Cantidad: (short)1, PrecioUnitario: 2500m, Notas: (string?)"Sin chantilly")
                }
            },
            new SpecPedido
            {
                IdUsuario = 4,
                TipoEntrega = "recoger",
                Estado = "cancelada",
                IdDireccion = null,
                FechaPedido = ahora.AddDays(-1),
                Notas = "Cliente canceló por demora",
                IdMetodoPago = 2,
                PagoEstado = "rechazado",
                MontoRecibido = null,
                ReferenciaPago = "SEED-TDC-5",
                Items =
                {
                    (IdProducto: 3, IdCombo: (int?)null, Cantidad: (short)2, PrecioUnitario: 4000m, Notas: (string?)null),
                    (IdProducto: (int?)null, IdCombo: 3, Cantidad: (short)1, PrecioUnitario: 7000m, Notas: (string?)null)
                }
            }
        };
    }

    private sealed class SpecPedido
    {
        public int IdUsuario { get; set; }
        public string TipoEntrega { get; set; } = "recoger";
        public string Estado { get; set; } = "pendiente_pago";
        public int? IdDireccion { get; set; }
        public DateTime FechaPedido { get; set; }
        public string? Notas { get; set; }
        public int IdMetodoPago { get; set; }
        public string PagoEstado { get; set; } = "pendiente";
        public decimal? MontoRecibido { get; set; }
        public string ReferenciaPago { get; set; } = "";
        public List<(int? IdProducto, int? IdCombo, short Cantidad, decimal PrecioUnitario, string? Notas)> Items { get; set; } = new();
    }
}