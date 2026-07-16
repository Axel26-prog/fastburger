using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Data;
using FastBurger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FastBurger.Application.Services;

public class ProductoService : IProductoService
{
    private readonly FastBurgerContext _context;

    public ProductoService(FastBurgerContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductoDTO>> GetAllAsync()
    {
        return await _context.Productos
            .Include(p => p.IdCategoriaNavigation)
            .Select(p => new ProductoDTO
            {
                IdProducto = p.IdProducto,
                IdCategoria = p.IdCategoria,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                ImagenUrl = p.ImagenUrl,
                Disponible = p.Disponible,
                TiempoPrepMin = p.TiempoPrepMin,
                Calorias = p.Calorias,
                NombreCategoria = p.IdCategoriaNavigation.Nombre
            })
            .ToListAsync();
    }

    public async Task<ProductoDTO?> GetByIdAsync(int id)
    {
        var producto = await _context.Productos
            .Include(p => p.IdCategoriaNavigation)
            .Include(p => p.ProductoIngredientes)
            .ThenInclude(pi => pi.IdIngredienteNavigation)
            .FirstOrDefaultAsync(p => p.IdProducto == id);

        if (producto == null) return null;

        return new ProductoDTO
        {
            IdProducto = producto.IdProducto,
            IdCategoria = producto.IdCategoria,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            ImagenUrl = producto.ImagenUrl,
            Disponible = producto.Disponible,
            TiempoPrepMin = producto.TiempoPrepMin,
            Calorias = producto.Calorias,
            NombreCategoria = producto.IdCategoriaNavigation.Nombre,
            Ingredientes = producto.ProductoIngredientes.Select(pi => pi.IdIngredienteNavigation.Nombre).ToList()
        };
    }

    public async Task<ProductoDTO> CreateAsync(CreateProductoDTO dto)
    {
        var producto = new Producto
        {
            IdCategoria = dto.IdCategoria,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            ImagenUrl = dto.ImagenUrl,
            Disponible = dto.Disponible,
            TiempoPrepMin = dto.TiempoPrepMin,
            Calorias = dto.Calorias
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(producto.IdProducto)!;
    }

    public async Task UpdateAsync(UpdateProductoDTO dto)
    {
        var producto = await _context.Productos.FindAsync(dto.IdProducto);
        if (producto == null) throw new Exception("Producto no encontrado");

        producto.IdCategoria = dto.IdCategoria;
        producto.Nombre = dto.Nombre;
        producto.Descripcion = dto.Descripcion;
        producto.Precio = dto.Precio;
        producto.ImagenUrl = dto.ImagenUrl;
        producto.Disponible = dto.Disponible;
        producto.TiempoPrepMin = dto.TiempoPrepMin;
        producto.Calorias = dto.Calorias;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) throw new Exception("Producto no encontrado");

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();
    }
}