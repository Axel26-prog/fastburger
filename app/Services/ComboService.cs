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
            Productos = combo.ComboProductos.Select(cp => cp.IdProductoNavigation.Nombre).ToList()
        };
    }

    public async Task<ComboDTO> CreateAsync(CreateComboDTO dto)
    {
        var combo = new Combo
        {
            IdCategoria = dto.IdCategoria,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            ImagenUrl = dto.ImagenUrl,
            Disponible = dto.Disponible,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin
        };

        _context.Combos.Add(combo);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(combo.IdCombo)!;
    }

    public async Task UpdateAsync(UpdateComboDTO dto)
    {
        var combo = await _context.Combos.FindAsync(dto.IdCombo);
        if (combo == null) throw new Exception("Combo no encontrado");

        combo.IdCategoria = dto.IdCategoria;
        combo.Nombre = dto.Nombre;
        combo.Descripcion = dto.Descripcion;
        combo.Precio = dto.Precio;
        combo.ImagenUrl = dto.ImagenUrl;
        combo.Disponible = dto.Disponible;
        combo.FechaInicio = dto.FechaInicio;
        combo.FechaFin = dto.FechaFin;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var combo = await _context.Combos.FindAsync(id);
        if (combo == null) throw new Exception("Combo no encontrado");

        _context.Combos.Remove(combo);
        await _context.SaveChangesAsync();
    }
}