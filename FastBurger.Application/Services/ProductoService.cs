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
            .Where(p => p.Disponible)
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

    public async Task<IEnumerable<ProductoDTO>> GetAllForMantenimientoAsync()
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
            Ingredientes = producto.ProductoIngredientes.Select(pi => pi.IdIngredienteNavigation.Nombre).ToList(),
            IngredienteIds = producto.ProductoIngredientes.Select(pi => pi.IdIngrediente).ToList()
        };
    }

    public async Task<ProductoDTO> CreateAsync(CreateProductoDTO dto)
    {
        if (await ExisteNombreAsync(dto.Nombre))
        {
            throw new InvalidOperationException("Ya existe un producto con este nombre.");
        }

        var producto = new Producto
        {
            IdCategoria = dto.IdCategoria,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            Disponible = dto.Disponible,
            TiempoPrepMin = dto.TiempoPrepMin,
            Calorias = dto.Calorias,
            ImagenUrl = dto.ImagenUrl
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        if (dto.IngredienteIds.Any())
        {
            foreach (var ingredienteId in dto.IngredienteIds)
            {
                var productoIngrediente = new ProductoIngrediente
                {
                    IdProducto = producto.IdProducto,
                    IdIngrediente = ingredienteId
                };
                _context.ProductoIngredientes.Add(productoIngrediente);
            }
            await _context.SaveChangesAsync();
        }

        return await GetByIdAsync(producto.IdProducto)!;
    }

    public async Task UpdateAsync(UpdateProductoDTO dto)
    {
        if (await ExisteNombreAsync(dto.Nombre, dto.IdProducto))
        {
            throw new InvalidOperationException("Ya existe un producto con este nombre.");
        }

        var producto = await _context.Productos
            .Include(p => p.ProductoIngredientes)
            .FirstOrDefaultAsync(p => p.IdProducto == dto.IdProducto);

        if (producto == null) throw new Exception("Producto no encontrado");

        producto.IdCategoria = dto.IdCategoria;
        producto.Nombre = dto.Nombre;
        producto.Descripcion = dto.Descripcion;
        producto.Precio = dto.Precio;
        producto.Disponible = dto.Disponible;
        producto.TiempoPrepMin = dto.TiempoPrepMin;
        producto.Calorias = dto.Calorias;

        if (!string.IsNullOrEmpty(dto.ImagenUrl))
        {
            producto.ImagenUrl = dto.ImagenUrl;
        }

        _context.ProductoIngredientes.RemoveRange(producto.ProductoIngredientes);

        if (dto.IngredienteIds.Any())
        {
            foreach (var ingredienteId in dto.IngredienteIds)
            {
                var productoIngrediente = new ProductoIngrediente
                {
                    IdProducto = producto.IdProducto,
                    IdIngrediente = ingredienteId
                };
                _context.ProductoIngredientes.Add(productoIngrediente);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? idProductoExcluir = null)
    {
        var query = _context.Productos.Where(p => p.Nombre == nombre);
        if (idProductoExcluir.HasValue)
        {
            query = query.Where(p => p.IdProducto != idProductoExcluir.Value);
        }
        return await query.AnyAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var producto = await _context.Productos
            .FirstOrDefaultAsync(p => p.IdProducto == id);

        if (producto == null) throw new Exception("Producto no encontrado");

        producto.Disponible = false;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoriaDTO>> GetCategoriasAsync()
    {
        return await _context.CategoriaProductos
            .Where(c => c.Activa)
            .Select(c => new CategoriaDTO
            {
                IdCategoria = c.IdCategoria,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Activa = c.Activa
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<IngredienteDTO>> GetIngredientesAsync()
    {
        return await _context.Ingredientes
            .Select(i => new IngredienteDTO
            {
                IdIngrediente = i.IdIngrediente,
                Nombre = i.Nombre,
                Descripcion = i.Descripcion,
                Alergenico = i.Alergenico,
                UnidadMedida = i.UnidadMedida
            })
            .ToListAsync();
    }
}