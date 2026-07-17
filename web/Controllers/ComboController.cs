using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

public class ComboController : Controller
{
    private readonly IComboService _comboService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ComboController(IComboService comboService, IWebHostEnvironment webHostEnvironment)
    {
        _comboService = comboService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var combos = await _comboService.GetAllAsync();
        return View(combos);
    }

    public async Task<IActionResult> Mantenimiento()
    {
        var combos = await _comboService.GetAllAsync();
        return View("Mantenimiento/Index", combos);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var combo = await _comboService.GetByIdAsync(id);
        if (combo == null) return NotFound();
        return View(combo);
    }

    public async Task<IActionResult> Crear()
    {
        var productos = await _comboService.GetProductosAsync();
        ViewBag.Productos = productos;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CreateComboDTO dto)
    {
        if (ModelState.IsValid)
        {
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "combos", fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    dto.ImagenUrl = $"/images/combos/{fileName}";
                }
            }

            await _comboService.CreateAsync(dto);
            return RedirectToAction(nameof(Mantenimiento));
        }

        var productos = await _comboService.GetProductosAsync();
        ViewBag.Productos = productos;
        return View(dto);
    }

    public async Task<IActionResult> Modificar(int id)
    {
        var combo = await _comboService.GetByIdAsync(id);
        if (combo == null) return NotFound();

        var comboVM = new UpdateComboDTO
        {
            IdCombo = combo.IdCombo,
            Nombre = combo.Nombre,
            Precio = combo.Precio,
            ImagenUrlActual = combo.ImagenUrl,
            Disponible = combo.Disponible
        };

        var productos = await _comboService.GetProductosAsync();
        ViewBag.Productos = productos;
        ViewBag.ProductosCombo = combo.Productos;

        return View(comboVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Modificar(UpdateComboDTO dto)
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
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "combos", fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    dto.ImagenUrl = $"/images/combos/{fileName}";
                }
            }
            else
            {
                dto.ImagenUrl = dto.ImagenUrlActual;
            }

            await _comboService.UpdateAsync(dto);
            return RedirectToAction(nameof(Mantenimiento));
        }

        var productos = await _comboService.GetProductosAsync();
        ViewBag.Productos = productos;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var combo = await _comboService.GetByIdAsync(id);
        if (combo != null && !string.IsNullOrEmpty(combo.ImagenUrl))
        {
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, combo.ImagenUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }

        await _comboService.DeleteAsync(id);
        return RedirectToAction(nameof(Mantenimiento));
    }
}