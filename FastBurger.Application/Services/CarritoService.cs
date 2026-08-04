using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Data;
using FastBurger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FastBurger.Application.Services;

public class CarritoService : ICarritoService
{
    private readonly FastBurgerContext _context;

    public CarritoService(FastBurgerContext context)
    {
        _context = context;
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

        return new ResumenCarritoDTO
        {
            IdCarrito = carrito.IdCarrito,
            IdUsuario = carrito.IdUsuario,
            Estado = carrito.Estado,
            TotalItems = carrito.CarritoItems.Sum(ci => ci.Cantidad),
            Subtotal = carrito.CarritoItems.Sum(ci => ci.Cantidad * ci.PrecioUnitario),
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

    public async Task<ResumenCarritoDTO> ObtenerOCrearCarritoAsync(int idUsuario)
    {
        var carrito = await _context.Carritos
            .Include(c => c.CarritoItems)
                .ThenInclude(ci => ci.IdProductoNavigation)
            .Include(c => c.CarritoItems)
                .ThenInclude(ci => ci.IdComboNavigation)
            .Where(c => c.IdUsuario == idUsuario && c.Estado == "activo")
            .FirstOrDefaultAsync();

        if (carrito == null)
        {
            carrito = new Carrito
            {
                IdUsuario = idUsuario,
                Estado = "activo",
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };
            _context.Carritos.Add(carrito);
            await _context.SaveChangesAsync();
        }

        return new ResumenCarritoDTO
        {
            IdCarrito = carrito.IdCarrito,
            IdUsuario = carrito.IdUsuario,
            Estado = carrito.Estado,
            TotalItems = carrito.CarritoItems.Sum(ci => ci.Cantidad),
            Subtotal = carrito.CarritoItems.Sum(ci => ci.Cantidad * ci.PrecioUnitario),
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

    public async Task<CarritoItemAgregadoDTO> AgregarProductoAsync(AgregarAlCarritoDTO dto)
    {
        if (dto.IdProducto == null && dto.IdCombo == null)
            throw new InvalidOperationException("Debe especificar un producto o combo.");

        if (dto.Cantidad <= 0)
            throw new InvalidOperationException("La cantidad debe ser mayor a cero.");

        var carrito = await _context.Carritos
            .Include(c => c.CarritoItems)
            .Where(c => c.IdUsuario == dto.IdUsuario && c.Estado == "activo")
            .FirstOrDefaultAsync();

        if (carrito == null)
        {
            carrito = new Carrito
            {
                IdUsuario = dto.IdUsuario,
                Estado = "activo",
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };
            _context.Carritos.Add(carrito);
            await _context.SaveChangesAsync();
        }

        decimal precioUnitario = 0;
        string? nombreProducto = null;
        string? nombreCombo = null;

        if (dto.IdProducto.HasValue)
        {
            var producto = await _context.Productos.FindAsync(dto.IdProducto.Value);
            if (producto == null)
                throw new InvalidOperationException("Producto no encontrado.");
            if (!producto.Disponible)
                throw new InvalidOperationException("El producto no está disponible.");

            precioUnitario = producto.Precio;
            nombreProducto = producto.Nombre;

            var itemExistente = carrito.CarritoItems
                .FirstOrDefault(ci => ci.IdProducto == dto.IdProducto.Value && string.IsNullOrEmpty(ci.Notas));

            if (itemExistente != null)
            {
                itemExistente.Cantidad += dto.Cantidad;
                carrito.FechaActualizacion = DateTime.Now;
                await _context.SaveChangesAsync();

                return new CarritoItemAgregadoDTO
                {
                    IdItem = itemExistente.IdItem,
                    IdCarrito = carrito.IdCarrito,
                    IdProducto = itemExistente.IdProducto,
                    NombreProducto = nombreProducto,
                    Cantidad = itemExistente.Cantidad,
                    PrecioUnitario = itemExistente.PrecioUnitario,
                    Subtotal = itemExistente.Cantidad * itemExistente.PrecioUnitario
                };
            }
        }
        else if (dto.IdCombo.HasValue)
        {
            var combo = await _context.Combos.FindAsync(dto.IdCombo.Value);
            if (combo == null)
                throw new InvalidOperationException("Combo no encontrado.");
            if (!combo.Disponible)
                throw new InvalidOperationException("El combo no está disponible.");

            precioUnitario = combo.Precio;
            nombreCombo = combo.Nombre;

            var itemExistente = carrito.CarritoItems
                .FirstOrDefault(ci => ci.IdCombo == dto.IdCombo.Value && string.IsNullOrEmpty(ci.Notas));

            if (itemExistente != null)
            {
                itemExistente.Cantidad += dto.Cantidad;
                carrito.FechaActualizacion = DateTime.Now;
                await _context.SaveChangesAsync();

                return new CarritoItemAgregadoDTO
                {
                    IdItem = itemExistente.IdItem,
                    IdCarrito = carrito.IdCarrito,
                    IdCombo = itemExistente.IdCombo,
                    NombreCombo = nombreCombo,
                    Cantidad = itemExistente.Cantidad,
                    PrecioUnitario = itemExistente.PrecioUnitario,
                    Subtotal = itemExistente.Cantidad * itemExistente.PrecioUnitario
                };
            }
        }

        var nuevoItem = new CarritoItem
        {
            IdCarrito = carrito.IdCarrito,
            IdProducto = dto.IdProducto,
            IdCombo = dto.IdCombo,
            Cantidad = dto.Cantidad,
            PrecioUnitario = precioUnitario,
            Notas = dto.Notas
        };

        _context.CarritoItems.Add(nuevoItem);
        carrito.FechaActualizacion = DateTime.Now;
        await _context.SaveChangesAsync();

        return new CarritoItemAgregadoDTO
        {
            IdItem = nuevoItem.IdItem,
            IdCarrito = carrito.IdCarrito,
            IdProducto = dto.IdProducto,
            NombreProducto = nombreProducto,
            IdCombo = dto.IdCombo,
            NombreCombo = nombreCombo,
            Cantidad = nuevoItem.Cantidad,
            PrecioUnitario = nuevoItem.PrecioUnitario,
            Subtotal = nuevoItem.Cantidad * nuevoItem.PrecioUnitario
        };
    }

    public async Task<bool> EliminarItemAsync(int idItem, int idUsuario)
    {
        var item = await _context.CarritoItems
            .Include(ci => ci.IdCarritoNavigation)
            .FirstOrDefaultAsync(ci => ci.IdItem == idItem && ci.IdCarritoNavigation.IdUsuario == idUsuario);

        if (item == null) return false;

        _context.CarritoItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> VaciarCarritoAsync(int idUsuario)
    {
        var carrito = await _context.Carritos
            .Include(c => c.CarritoItems)
            .Where(c => c.IdUsuario == idUsuario && c.Estado == "activo")
            .FirstOrDefaultAsync();

        if (carrito == null) return false;

        _context.CarritoItems.RemoveRange(carrito.CarritoItems);
        carrito.FechaActualizacion = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActualizarCantidadAsync(int idItem, short cantidad, int idUsuario)
    {
        var item = await _context.CarritoItems
            .Include(ci => ci.IdCarritoNavigation)
            .FirstOrDefaultAsync(ci => ci.IdItem == idItem && ci.IdCarritoNavigation.IdUsuario == idUsuario);

        if (item == null) return false;

        if (cantidad <= 0)
        {
            _context.CarritoItems.Remove(item);
        }
        else
        {
            item.Cantidad = cantidad;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
