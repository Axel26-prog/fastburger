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
        var productos = await _productoService.GetAllForMantenimientoAsync();
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
            try
            {
                if (Request.Form.Files.Count == 0 || Request.Form.Files[0].Length == 0)
                {
                    ModelState.AddModelError("Nombre", "Debe subir una imagen del producto.");
                    var cats = await _productoService.GetCategoriasAsync();
                    var ings = await _productoService.GetIngredientesAsync();
                    ViewBag.Categorias = cats;
                    ViewBag.Ingredientes = ings;
                    return View(dto);
                }

                var file = Request.Form.Files[0];
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Nombre", "El archivo debe ser una imagen válida (JPG, PNG, GIF o WebP).");
                    var cats = await _productoService.GetCategoriasAsync();
                    var ings = await _productoService.GetIngredientesAsync();
                    ViewBag.Categorias = cats;
                    ViewBag.Ingredientes = ings;
                    return View(dto);
                }

                const long maxSize = 5 * 1024 * 1024; // 5MB
                if (file.Length > maxSize)
                {
                    ModelState.AddModelError("Nombre", "La imagen no puede superar los 5 MB.");
                    var cats = await _productoService.GetCategoriasAsync();
                    var ings = await _productoService.GetIngredientesAsync();
                    ViewBag.Categorias = cats;
                    ViewBag.Ingredientes = ings;
                    return View(dto);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "productos", fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);
                dto.ImagenUrl = $"/images/productos/{fileName}";

                await _productoService.CreateAsync(dto);
                return RedirectToAction(nameof(Mantenimiento));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Nombre", ex.Message);
            }
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
            Calorias = producto.Calorias,
            IngredienteIds = producto.IngredienteIds
        };

        var categorias = await _productoService.GetCategoriasAsync();
        var ingredientes = await _productoService.GetIngredientesAsync();
        ViewBag.Categorias = categorias;
        ViewBag.Ingredientes = ingredientes;
        ViewBag.IngredientesProducto = producto.IngredienteIds;

        return View(productoVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Modificar(UpdateProductoDTO dto)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    if (file.Length > 0)
                    {
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("Nombre", "El archivo debe ser una imagen válida (JPG, PNG, GIF o WebP).");
                            var cats = await _productoService.GetCategoriasAsync();
                            var ings = await _productoService.GetIngredientesAsync();
                            ViewBag.Categorias = cats;
                            ViewBag.Ingredientes = ings;
                            return View(dto);
                        }

                        const long maxSize = 5 * 1024 * 1024; // 5MB
                        if (file.Length > maxSize)
                        {
                            ModelState.AddModelError("Nombre", "La imagen no puede superar los 5 MB.");
                            var cats = await _productoService.GetCategoriasAsync();
                            var ings = await _productoService.GetIngredientesAsync();
                            ViewBag.Categorias = cats;
                            ViewBag.Ingredientes = ings;
                            return View(dto);
                        }

                        if (!string.IsNullOrEmpty(dto.ImagenUrlActual))
                        {
                            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, dto.ImagenUrlActual.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);
                        }

                        var fileName = $"{Guid.NewGuid()}{extension}";
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "productos", fileName);
                        using var stream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(stream);
                        dto.ImagenUrl = $"/images/productos/{fileName}";
                    }
                    else
                    {
                        dto.ImagenUrl = dto.ImagenUrlActual;
                    }
                }
                else
                {
                    dto.ImagenUrl = dto.ImagenUrlActual;
                }

                await _productoService.UpdateAsync(dto);
                return RedirectToAction(nameof(Mantenimiento));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Nombre", ex.Message);
            }
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
        await _productoService.DeleteAsync(id);
        return RedirectToAction(nameof(Mantenimiento));
    }
}