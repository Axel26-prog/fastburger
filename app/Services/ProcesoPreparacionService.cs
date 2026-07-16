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
            .Select(p => new ProcesoPreparacionDTO
            {
                IdProceso = p.IdProceso,
                IdProducto = p.IdProducto,
                Descripcion = p.Descripcion,
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
            NombreProducto = proceso.IdProductoNavigation.Nombre,
            CantidadPasos = proceso.PasoPreparacions.Count,
            Pasos = proceso.PasoPreparacions
                .OrderBy(pp => pp.Orden)
                .Select(pp => new PasoDTO
                {
                    Orden = pp.Orden,
                    NombreEstacion = pp.IdEstacionNavigation.Nombre
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

        return await GetByIdAsync(proceso.IdProceso)!;
    }

    public async Task UpdateAsync(UpdateProcesoPreparacionDTO dto)
    {
        var proceso = await _context.ProcesoPreparacions.FindAsync(dto.IdProceso);
        if (proceso == null) throw new Exception("Proceso de preparacion no encontrado");

        proceso.IdProducto = dto.IdProducto;
        proceso.Descripcion = dto.Descripcion;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var proceso = await _context.ProcesoPreparacions.FindAsync(id);
        if (proceso == null) throw new Exception("Proceso de preparacion no encontrado");

        _context.ProcesoPreparacions.Remove(proceso);
        await _context.SaveChangesAsync();
    }
}