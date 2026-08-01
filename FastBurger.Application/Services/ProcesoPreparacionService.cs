using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Data;
using FastBurger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FastBurger.Application.Services;

public class ProcesoPreparacionService : IProcesoPreparacionService
{
    private readonly FastBurgerContext _context;

    public ProcesoPreparacionService(FastBurgerContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProcesoPreparacionDTO>> GetAllAsync()
    {
        return await _context.ProcesoPreparacions
            .Include(p => p.IdProductoNavigation)
            .Include(p => p.PasoPreparacions)
            .Where(p => p.Activo)
            .Select(p => new ProcesoPreparacionDTO
            {
                IdProceso = p.IdProceso,
                IdProducto = p.IdProducto,
                Descripcion = p.Descripcion,
                Activo = p.Activo,
                NombreProducto = p.IdProductoNavigation.Nombre,
                CantidadPasos = p.PasoPreparacions.Count
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ProcesoPreparacionDTO>> GetAllForMantenimientoAsync()
    {
        return await _context.ProcesoPreparacions
            .Include(p => p.IdProductoNavigation)
            .Include(p => p.PasoPreparacions)
            .Select(p => new ProcesoPreparacionDTO
            {
                IdProceso = p.IdProceso,
                IdProducto = p.IdProducto,
                Descripcion = p.Descripcion,
                Activo = p.Activo,
                NombreProducto = p.IdProductoNavigation.Nombre,
                CantidadPasos = p.PasoPreparacions.Count
            })
            .ToListAsync();
    }

    public async Task<ProcesoPreparacionDTO?> GetByIdAsync(int id)
    {
        var proceso = await _context.ProcesoPreparacions
            .Include(p => p.IdProductoNavigation)
            .Include(p => p.PasoPreparacions)
            .ThenInclude(pp => pp.IdEstacionNavigation)
            .FirstOrDefaultAsync(p => p.IdProceso == id);

        if (proceso == null) return null;

        return new ProcesoPreparacionDTO
        {
            IdProceso = proceso.IdProceso,
            IdProducto = proceso.IdProducto,
            Descripcion = proceso.Descripcion,
            Activo = proceso.Activo,
            NombreProducto = proceso.IdProductoNavigation.Nombre,
            CantidadPasos = proceso.PasoPreparacions.Count,
            Pasos = proceso.PasoPreparacions
                .OrderBy(pp => pp.Orden)
                .Select(pp => new PasoDTO
                {
                    IdPaso = pp.IdPaso,
                    Orden = pp.Orden,
                    NombreEstacion = pp.IdEstacionNavigation.Nombre,
                    IdEstacion = pp.IdEstacion,
                    Descripcion = pp.Descripcion,
                    TiempoMin = pp.TiempoMin,
                    TemperaturaC = pp.TemperaturaC
                }).ToList()
        };
    }

    public async Task<ProcesoPreparacionDTO> CreateAsync(CreateProcesoPreparacionDTO dto)
    {
        var proceso = new ProcesoPreparacion
        {
            IdProducto = dto.IdProducto,
            Descripcion = dto.Descripcion
        };

        _context.ProcesoPreparacions.Add(proceso);
        await _context.SaveChangesAsync();

        if (dto.Pasos.Any())
        {
            foreach (var paso in dto.Pasos)
            {
                var pasoPreparacion = new PasoPreparacion
                {
                    IdProceso = proceso.IdProceso,
                    IdEstacion = paso.IdEstacion,
                    Orden = (short)paso.Orden,
                    Descripcion = paso.Descripcion,
                    TiempoMin = paso.TiempoMin,
                    TemperaturaC = paso.TemperaturaC
                };
                _context.PasoPreparacions.Add(pasoPreparacion);
            }
            await _context.SaveChangesAsync();
        }

        return await GetByIdAsync(proceso.IdProceso)!;
    }

    public async Task UpdateAsync(UpdateProcesoPreparacionDTO dto)
    {
        var proceso = await _context.ProcesoPreparacions
            .Include(p => p.PasoPreparacions)
            .FirstOrDefaultAsync(p => p.IdProceso == dto.IdProceso);

        if (proceso == null) throw new Exception("Proceso de preparacion no encontrado");

        proceso.IdProducto = dto.IdProducto;
        proceso.Descripcion = dto.Descripcion;

        _context.PasoPreparacions.RemoveRange(proceso.PasoPreparacions);

        if (dto.Pasos.Any())
        {
            foreach (var paso in dto.Pasos)
            {
                var pasoPreparacion = new PasoPreparacion
                {
                    IdProceso = proceso.IdProceso,
                    IdEstacion = paso.IdEstacion,
                    Orden = (short)paso.Orden,
                    Descripcion = paso.Descripcion,
                    TiempoMin = paso.TiempoMin,
                    TemperaturaC = paso.TemperaturaC
                };
                _context.PasoPreparacions.Add(pasoPreparacion);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var proceso = await _context.ProcesoPreparacions
            .FirstOrDefaultAsync(p => p.IdProceso == id);

        if (proceso == null) throw new Exception("Proceso de preparacion no encontrado");

        proceso.Activo = false;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<DTOs.ProductoSimpleDTO>> GetProductosDisponiblesAsync()
    {
        var productosConProceso = await _context.ProcesoPreparacions
            .Select(p => p.IdProducto)
            .Distinct()
            .ToListAsync();

        return await _context.Productos
            .Where(p => !productosConProceso.Contains(p.IdProducto))
            .Select(p => new DTOs.ProductoSimpleDTO
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<DTOs.EstacionSimpleDTO>> GetEstacionesAsync()
    {
        return await _context.EstacionCocinas
            .Where(e => e.Activa)
            .Select(e => new DTOs.EstacionSimpleDTO
            {
                IdEstacion = e.IdEstacion,
                Nombre = e.Nombre
            })
            .ToListAsync();
    }
}