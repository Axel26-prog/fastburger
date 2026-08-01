using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Data;
using FastBurger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FastBurger.Application.Services;

public class ComboService : IComboService
{
    private readonly FastBurgerContext _context;

    public ComboService(FastBurgerContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ComboDTO>> GetAllAsync()
    {
        return await _context.Combos
            .Include(c => c.IdCategoriaNavigation)
            .Include(c => c.ComboProductos)
            .ThenInclude(cp => cp.IdProductoNavigation)
            .Where(c => c.Disponible)
            .Select(c => new ComboDTO
            {
                IdCombo = c.IdCombo,
                IdCategoria = c.IdCategoria,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Precio = c.Precio,
                ImagenUrl = c.ImagenUrl,
                Disponible = c.Disponible,
                FechaInicio = c.FechaInicio,
                FechaFin = c.FechaFin,
                NombreCategoria = c.IdCategoriaNavigation.Nombre,
                Productos = c.ComboProductos.Select(cp => cp.IdProductoNavigation.Nombre).ToList()
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ComboDTO>> GetAllForMantenimientoAsync()
    {
        return await _context.Combos
            .Include(c => c.IdCategoriaNavigation)
            .Include(c => c.ComboProductos)
            .ThenInclude(cp => cp.IdProductoNavigation)
            .Select(c => new ComboDTO
            {
                IdCombo = c.IdCombo,
                IdCategoria = c.IdCategoria,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Precio = c.Precio,
                ImagenUrl = c.ImagenUrl,
                Disponible = c.Disponible,
                FechaInicio = c.FechaInicio,
                FechaFin = c.FechaFin,
                NombreCategoria = c.IdCategoriaNavigation.Nombre,
                Productos = c.ComboProductos.Select(cp => cp.IdProductoNavigation.Nombre).ToList()
            })
            .ToListAsync();
    }

    public async Task<ComboDTO?> GetByIdAsync(int id)
    {
        var combo = await _context.Combos
            .Include(c => c.IdCategoriaNavigation)
            .Include(c => c.ComboProductos)
            .ThenInclude(cp => cp.IdProductoNavigation)
            .FirstOrDefaultAsync(c => c.IdCombo == id);

        if (combo == null) return null;

        return new ComboDTO
        {
            IdCombo = combo.IdCombo,
            IdCategoria = combo.IdCategoria,
            Nombre = combo.Nombre,
            Descripcion = combo.Descripcion,
            Precio = combo.Precio,
            ImagenUrl = combo.ImagenUrl,
            Disponible = combo.Disponible,
            FechaInicio = combo.FechaInicio,
            FechaFin = combo.FechaFin,
            NombreCategoria = combo.IdCategoriaNavigation.Nombre,
            Productos = combo.ComboProductos.Select(cp => cp.IdProductoNavigation.Nombre).ToList(),
            ProductoIds = combo.ComboProductos.Select(cp => cp.IdProducto).ToList()
        };
    }

    public async Task<ComboDTO> CreateAsync(CreateComboDTO dto)
    {
        var combo = new Combo
        {
            Nombre = dto.Nombre,
            Precio = dto.Precio,
            Disponible = dto.Disponible,
            IdCategoria = 1
        };

        _context.Combos.Add(combo);
        await _context.SaveChangesAsync();

        if (dto.ProductoIds.Any())
        {
            foreach (var productoId in dto.ProductoIds)
            {
                var comboProducto = new ComboProducto
                {
                    IdCombo = combo.IdCombo,
                    IdProducto = productoId
                };
                _context.ComboProductos.Add(comboProducto);
            }
            await _context.SaveChangesAsync();
        }

        return await GetByIdAsync(combo.IdCombo)!;
    }

    public async Task UpdateAsync(UpdateComboDTO dto)
    {
        var combo = await _context.Combos
            .Include(c => c.ComboProductos)
            .FirstOrDefaultAsync(c => c.IdCombo == dto.IdCombo);

        if (combo == null) throw new Exception("Combo no encontrado");

        combo.Nombre = dto.Nombre;
        combo.Precio = dto.Precio;
        combo.Disponible = dto.Disponible;

        if (!string.IsNullOrEmpty(dto.ImagenUrl))
        {
            combo.ImagenUrl = dto.ImagenUrl;
        }

        _context.ComboProductos.RemoveRange(combo.ComboProductos);

        if (dto.ProductoIds.Any())
        {
            foreach (var productoId in dto.ProductoIds)
            {
                var comboProducto = new ComboProducto
                {
                    IdCombo = combo.IdCombo,
                    IdProducto = productoId
                };
                _context.ComboProductos.Add(comboProducto);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var combo = await _context.Combos
            .FirstOrDefaultAsync(c => c.IdCombo == id);

        if (combo == null) throw new Exception("Combo no encontrado");

        combo.Disponible = false;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<DTOs.ProductoSimpleDTO>> GetProductosAsync()
    {
        return await _context.Productos
            .Where(p => p.Disponible)
            .Select(p => new DTOs.ProductoSimpleDTO
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                Precio = p.Precio
            })
            .ToListAsync();
    }
}