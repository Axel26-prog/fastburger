using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

public class ProductoController : Controller
{
    private readonly IProductoService _productoService;

    public ProductoController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    public async Task<IActionResult> Index()
    {
        var productos = await _productoService.GetAllAsync();
        return View(productos);
    }

    public async Task<IActionResult> Listar()
    {
        var productos = await _productoService.GetAllAsync();
        return View(productos);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    public async Task<IActionResult> Crear()
    {
        var categorias = await _productoService.GetCategoriasAsync();
        var ingredientes = await _productoService.GetIngredientesAsync();
        var model = new CreateProductoDTO();
        ViewBag.Categorias = categorias;
        ViewBag.Ingredientes = ingredientes;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CreateProductoDTO dto)
    {
        if (ModelState.IsValid)
        {
            await _productoService.CreateAsync(dto);
            return RedirectToAction(nameof(Listar));
        }
        var categorias = await _productoService.GetCategoriasAsync();
        var ingredientes = await _productoService.GetIngredientesAsync();
        ViewBag.Categorias = categorias;
        ViewBag.Ingredientes = ingredientes;
        return View(dto);
    }

    public async Task<IActionResult> Editar(int id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();

        var updateDto = new UpdateProductoDTO
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
            IngredientesIds = producto.IngredientesIds
        };

        var categorias = await _productoService.GetCategoriasAsync();
        var ingredientes = await _productoService.GetIngredientesAsync();
        ViewBag.Categorias = categorias;
        ViewBag.Ingredientes = ingredientes;
        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(UpdateProductoDTO dto)
    {
        if (ModelState.IsValid)
        {
            await _productoService.UpdateAsync(dto);
            return RedirectToAction(nameof(Listar));
        }
        var categorias = await _productoService.GetCategoriasAsync();
        var ingredientes = await _productoService.GetIngredientesAsync();
        ViewBag.Categorias = categorias;
        ViewBag.Ingredientes = ingredientes;
        return View(dto);
    }
}