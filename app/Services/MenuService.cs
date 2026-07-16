using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Data;
using FastBurger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FastBurger.Application.Services;

public class MenuService : IMenuService
{
    private readonly FastBurgerContext _context;

    public MenuService(FastBurgerContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MenuDTO>> GetAllAsync()
    {
        return await _context.Menus
            .OrderByDescending(m => m.FechaInicio)
            .Select(m => new MenuDTO
            {
                IdMenu = m.IdMenu,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                Activo = m.Activo,
                FechaInicio = m.FechaInicio,
                FechaFin = m.FechaFin,
                HoraInicio = m.HoraInicio,
                HoraFin = m.HoraFin,
                DiasSemana = m.DiasSemana
            })
            .ToListAsync();
    }

    public async Task<MenuDTO?> GetDisponibleAsync()
    {
        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var ahora = TimeOnly.FromDateTime(DateTime.Now);

        var menu = await _context.Menus
            .Include(m => m.MenuProductos).ThenInclude(mp => mp.IdProductoNavigation).ThenInclude(p => p.IdCategoriaNavigation)
            .Include(m => m.MenuCombos).ThenInclude(mc => mc.IdComboNavigation).ThenInclude(c => c.IdCategoriaNavigation)
            .Where(m => m.Activo
                && (!m.FechaInicio.HasValue || m.FechaInicio.Value <= hoy)
                && (!m.FechaFin.HasValue || m.FechaFin.Value >= hoy)
                && (!m.HoraInicio.HasValue || m.HoraInicio.Value <= ahora)
                && (!m.HoraFin.HasValue || m.HoraFin.Value >= ahora))
            .FirstOrDefaultAsync();

        if (menu == null) return null;

        var items = new List<MenuItemDTO>();

        items.AddRange(menu.MenuProductos.Select(mp => new MenuItemDTO
        {
            Nombre = mp.IdProductoNavigation.Nombre,
            Precio = mp.PrecioEspecial ?? mp.IdProductoNavigation.Precio,
            ImagenUrl = mp.IdProductoNavigation.ImagenUrl,
            Categoria = mp.IdProductoNavigation.IdCategoriaNavigation.Nombre,
            Tipo = "Producto"
        }));

        items.AddRange(menu.MenuCombos.Select(mc => new MenuItemDTO
        {
            Nombre = mc.IdComboNavigation.Nombre,
            Precio = mc.PrecioEspecial ?? mc.IdComboNavigation.Precio,
            ImagenUrl = mc.IdComboNavigation.ImagenUrl,
            Categoria = mc.IdComboNavigation.IdCategoriaNavigation.Nombre,
            Tipo = "Combo"
        }));

        return new MenuDTO
        {
            IdMenu = menu.IdMenu,
            Nombre = menu.Nombre,
            Descripcion = menu.Descripcion,
            Activo = menu.Activo,
            FechaInicio = menu.FechaInicio,
            FechaFin = menu.FechaFin,
            HoraInicio = menu.HoraInicio,
            HoraFin = menu.HoraFin,
            DiasSemana = menu.DiasSemana,
            Items = items
        };
    }

    public async Task<MenuDTO?> GetByIdAsync(int id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu == null) return null;

        return new MenuDTO
        {
            IdMenu = menu.IdMenu,
            Nombre = menu.Nombre,
            Descripcion = menu.Descripcion,
            Activo = menu.Activo,
            FechaInicio = menu.FechaInicio,
            FechaFin = menu.FechaFin,
            HoraInicio = menu.HoraInicio,
            HoraFin = menu.HoraFin,
            DiasSemana = menu.DiasSemana
        };
    }

    public async Task<MenuDTO> CreateAsync(CreateMenuDTO dto)
    {
        var menu = new Menu
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Activo = dto.Activo,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            HoraInicio = dto.HoraInicio,
            HoraFin = dto.HoraFin,
            DiasSemana = dto.DiasSemana
        };

        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(menu.IdMenu)!;
    }

    public async Task UpdateAsync(UpdateMenuDTO dto)
    {
        var menu = await _context.Menus.FindAsync(dto.IdMenu);
        if (menu == null) throw new Exception("Menu no encontrado");

        menu.Nombre = dto.Nombre;
        menu.Descripcion = dto.Descripcion;
        menu.Activo = dto.Activo;
        menu.FechaInicio = dto.FechaInicio;
        menu.FechaFin = dto.FechaFin;
        menu.HoraInicio = dto.HoraInicio;
        menu.HoraFin = dto.HoraFin;
        menu.DiasSemana = dto.DiasSemana;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu == null) throw new Exception("Menu no encontrado");

        _context.Menus.Remove(menu);
        await _context.SaveChangesAsync();
    }
}