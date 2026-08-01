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

    private static readonly Dictionary<string, int> DiasOrden = new()
    {
        { "Lunes", 1 }, { "Martes", 2 }, { "Miércoles", 3 }, { "Jueves", 4 },
        { "Viernes", 5 }, { "Sábado", 6 }, { "Domingo", 7 }
    };

    private static bool DiaEstaEnRango(string? diasSemana, string diaActual)
    {
        if (string.IsNullOrWhiteSpace(diasSemana)) return true;

        diasSemana = diasSemana.Trim();
        if (string.IsNullOrEmpty(diasSemana)) return true;

        if (!DiasOrden.TryGetValue(diaActual, out int ordenDiaActual)) return false;

        var partes = diasSemana.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var parte in partes)
        {
            var texto = parte.Trim();

            if (texto.Contains('-'))
            {
                var rango = texto.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (rango.Length == 2)
                {
                    var diaInicio = NormalizarDia(rango[0]);
                    var diaFin = NormalizarDia(rango[1]);
                    if (DiasOrden.TryGetValue(diaInicio, out int ordenInicio) && DiasOrden.TryGetValue(diaFin, out int ordenFin))
                    {
                        if (ordenInicio <= ordenFin)
                        {
                            if (ordenDiaActual >= ordenInicio && ordenDiaActual <= ordenFin) return true;
                        }
                        else
                        {
                            if (ordenDiaActual >= ordenInicio || ordenDiaActual <= ordenFin) return true;
                        }
                    }
                }
            }
            else
            {
                var diaNormalizado = NormalizarDia(texto);
                if (DiasOrden.TryGetValue(diaNormalizado, out int orden) && orden == ordenDiaActual)
                    return true;
            }
        }

        return false;
    }

    private static string NormalizarDia(string dia)
    {
        dia = dia.Trim().ToLowerInvariant();
        return dia switch
        {
            "lun" or "lunes" => "Lunes",
            "mar" or "martes" => "Martes",
            "mié" or "mie" or "miércoles" => "Miércoles",
            "jue" or "jueves" => "Jueves",
            "vie" or "viernes" => "Viernes",
            "sáb" or "sab" or "sábado" => "Sábado",
            "dom" or "domingo" => "Domingo",
            _ => dia
        };
    }

    public async Task<IEnumerable<MenuDTO>> GetAllAsync()
    {
        return await _context.Menus
            .Where(m => m.Activo)
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

    public async Task<IEnumerable<MenuDTO>> GetAllForMantenimientoAsync()
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

        var diaActualEspanol = DateTime.Now.DayOfWeek switch
        {
            DayOfWeek.Monday => "Lunes",
            DayOfWeek.Tuesday => "Martes",
            DayOfWeek.Wednesday => "Miércoles",
            DayOfWeek.Thursday => "Jueves",
            DayOfWeek.Friday => "Viernes",
            DayOfWeek.Saturday => "Sábado",
            DayOfWeek.Sunday => "Domingo",
            _ => ""
        };

        var menus = await _context.Menus
            .Include(m => m.MenuProductos).ThenInclude(mp => mp.IdProductoNavigation).ThenInclude(p => p.IdCategoriaNavigation)
            .Include(m => m.MenuCombos).ThenInclude(mc => mc.IdComboNavigation).ThenInclude(c => c.IdCategoriaNavigation)
            .Where(m => m.Activo
                && (!m.FechaInicio.HasValue || m.FechaInicio.Value <= hoy)
                && (!m.FechaFin.HasValue || m.FechaFin.Value >= hoy)
                && (!m.HoraInicio.HasValue || m.HoraInicio.Value <= ahora)
                && (!m.HoraFin.HasValue || m.HoraFin.Value >= ahora))
            .ToListAsync();

        // Filtrar por día de la semana si está especificado
        // Si DiasSemana está vacío o null, aplica para todos los días
        if (!string.IsNullOrEmpty(diaActualEspanol))
        {
            menus = menus.Where(m => DiaEstaEnRango(m.DiasSemana, diaActualEspanol)).ToList();
        }

        if (!menus.Any()) return null;

        // Prioridad: menor duración = más específico = mayor prioridad
        // Desempate: IdMenu mayor (más reciente)
        var menu = menus
            .OrderBy(m => (m.HoraFin.HasValue && m.HoraInicio.HasValue)
                ? m.HoraFin.Value.ToTimeSpan().Subtract(m.HoraInicio.Value.ToTimeSpan()).TotalMinutes
                : int.MaxValue)
            .ThenByDescending(m => m.IdMenu)
            .First();

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
        var menu = await _context.Menus
            .Include(m => m.MenuProductos).ThenInclude(mp => mp.IdProductoNavigation).ThenInclude(p => p.IdCategoriaNavigation)
            .Include(m => m.MenuCombos).ThenInclude(mc => mc.IdComboNavigation).ThenInclude(c => c.IdCategoriaNavigation)
            .FirstOrDefaultAsync(m => m.IdMenu == id);

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

    public async Task<MenuForEditDTO?> GetByIdForEditAsync(int id)
    {
        var menu = await _context.Menus
            .Include(m => m.MenuProductos).ThenInclude(mp => mp.IdProductoNavigation)
            .Include(m => m.MenuCombos).ThenInclude(mc => mc.IdComboNavigation)
            .FirstOrDefaultAsync(m => m.IdMenu == id);

        if (menu == null) return null;

        return new MenuForEditDTO
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
            ProductoIds = menu.MenuProductos.Select(mp => mp.IdProducto).ToList(),
            ComboIds = menu.MenuCombos.Select(mc => mc.IdCombo).ToList(),
            PreciosProductos = menu.MenuProductos
                .Where(mp => mp.PrecioEspecial.HasValue)
                .ToDictionary(mp => mp.IdProducto, mp => mp.PrecioEspecial),
            PreciosCombos = menu.MenuCombos
                .Where(mc => mc.PrecioEspecial.HasValue)
                .ToDictionary(mc => mc.IdCombo, mc => mc.PrecioEspecial)
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

        if (dto.ProductoIds.Any())
        {
            foreach (var productoId in dto.ProductoIds)
            {
                var menuProducto = new MenuProducto
                {
                    IdMenu = menu.IdMenu,
                    IdProducto = productoId,
                    PrecioEspecial = dto.PreciosProductos.TryGetValue(productoId, out var pp) ? pp : null
                };
                _context.MenuProductos.Add(menuProducto);
            }
        }

        if (dto.ComboIds.Any())
        {
            foreach (var comboId in dto.ComboIds)
            {
                var menuCombo = new MenuCombo
                {
                    IdMenu = menu.IdMenu,
                    IdCombo = comboId,
                    PrecioEspecial = dto.PreciosCombos.TryGetValue(comboId, out var pc) ? pc : null
                };
                _context.MenuCombos.Add(menuCombo);
            }
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(menu.IdMenu)!;
    }

    public async Task UpdateAsync(UpdateMenuDTO dto)
    {
        var menu = await _context.Menus
            .Include(m => m.MenuProductos)
            .Include(m => m.MenuCombos)
            .FirstOrDefaultAsync(m => m.IdMenu == dto.IdMenu);

        if (menu == null) throw new Exception("Menu no encontrado");

        menu.Nombre = dto.Nombre;
        menu.Descripcion = dto.Descripcion;
        menu.Activo = dto.Activo;
        menu.FechaInicio = dto.FechaInicio;
        menu.FechaFin = dto.FechaFin;
        menu.HoraInicio = dto.HoraInicio;
        menu.HoraFin = dto.HoraFin;
        menu.DiasSemana = dto.DiasSemana;

        _context.MenuProductos.RemoveRange(menu.MenuProductos);
        _context.MenuCombos.RemoveRange(menu.MenuCombos);

        if (dto.ProductoIds.Any())
        {
            foreach (var productoId in dto.ProductoIds)
            {
                var menuProducto = new MenuProducto
                {
                    IdMenu = menu.IdMenu,
                    IdProducto = productoId,
                    PrecioEspecial = dto.PreciosProductos.TryGetValue(productoId, out var pp) ? pp : null
                };
                _context.MenuProductos.Add(menuProducto);
            }
        }

        if (dto.ComboIds.Any())
        {
            foreach (var comboId in dto.ComboIds)
            {
                var menuCombo = new MenuCombo
                {
                    IdMenu = menu.IdMenu,
                    IdCombo = comboId,
                    PrecioEspecial = dto.PreciosCombos.TryGetValue(comboId, out var pc) ? pc : null
                };
                _context.MenuCombos.Add(menuCombo);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var menu = await _context.Menus
            .FirstOrDefaultAsync(m => m.IdMenu == id);

        if (menu == null) throw new Exception("Menu no encontrado");

        menu.Activo = false;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<DTOs.ProductoSimpleDTO>> GetProductosDisponiblesAsync()
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

    public async Task<IEnumerable<DTOs.ComboSimpleDTO>> GetCombosDisponiblesAsync()
    {
        return await _context.Combos
            .Where(c => c.Disponible)
            .Select(c => new DTOs.ComboSimpleDTO
            {
                IdCombo = c.IdCombo,
                Nombre = c.Nombre,
                Precio = c.Precio
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<DTOs.ProductoSimpleDTO>> GetAllProductosAsync()
    {
        return await _context.Productos
            .Select(p => new DTOs.ProductoSimpleDTO
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                Precio = p.Precio
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<DTOs.ComboSimpleDTO>> GetAllCombosAsync()
    {
        return await _context.Combos
            .Select(c => new DTOs.ComboSimpleDTO
            {
                IdCombo = c.IdCombo,
                Nombre = c.Nombre,
                Precio = c.Precio
            })
            .ToListAsync();
    }
}