using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

public class ProductoController : Controller
{
    private readonly IProductoService _productoService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductoController(IProductoService productoService, IWebHostEnvironment webHostEnvironment)
    {
        _productoService = productoService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
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

    public async Task<IActionResult> Mantenimiento()
    {
        var productos = await _productoService.GetAllAsync();
        return View("Mantenimiento/Index", productos);
    }

    public async Task<IActionResult> Crear()
    {
        var categorias = await _productoService.GetCategoriasAsync();
        var ingredientes = await _productoService.GetIngredientesAsync();
        ViewBag.Categorias = categorias;
        ViewBag.Ingredientes = ingredientes;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CreateProductoDTO dto)
    {
        if (ModelState.IsValid)
        {
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "productos", fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    dto.ImagenUrl = $"/images/productos/{fileName}";
                }
            }

            await _productoService.CreateAsync(dto);
            return RedirectToAction(nameof(Mantenimiento));
        }

        var categorias = await _productoService.GetCategoriasAsync();
        var ingredientes = await _productoService.GetIngredientesAsync();
        ViewBag.Categorias = categorias;
        ViewBag.Ingredientes = ingredientes;
        return View(dto);
    }

    public async Task<IActionResult> Modificar(int id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();

        var productoVM = new UpdateProductoDTO
        {
            IdProducto = producto.IdProducto,
            IdCategoria = producto.IdCategoria,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            ImagenUrlActual = producto.ImagenUrl,
            Disponible = producto.Disponible,
            TiempoPrepMin = producto.TiempoPrepMin,
            Calorias = producto.Calorias
        };

        var categorias = await _productoService.GetCategoriasAsync();
        var ingredientes = await _productoService.GetIngredientesAsync();
        ViewBag.Categorias = categorias;
        ViewBag.Ingredientes = ingredientes;
        ViewBag.IngredientesProducto = producto.Ingredientes;

        return View(productoVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Modificar(UpdateProductoDTO dto)
    {
        if (ModelState.IsValid)
        {
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    if (!string.IsNullOrEmpty(dto.ImagenUrlActual))
                    {
                        var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, dto.ImagenUrlActual.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "productos", fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    dto.ImagenUrl = $"/images/productos/{fileName}";
                }
            }
            else
            {
                dto.ImagenUrl = dto.ImagenUrlActual;
            }

            await _productoService.UpdateAsync(dto);
            return RedirectToAction(nameof(Mantenimiento));
        }

        var categorias = await _productoService.GetCategoriasAsync();
        var ingredientes = await _productoService.GetIngredientesAsync();
        ViewBag.Categorias = categorias;
        ViewBag.Ingredientes = ingredientes;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        if (producto != null && !string.IsNullOrEmpty(producto.ImagenUrl))
        {
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, producto.ImagenUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }

        await _productoService.DeleteAsync(id);
        return RedirectToAction(nameof(Mantenimiento));
    }
}