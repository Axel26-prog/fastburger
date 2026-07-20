using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

public class MenuController : Controller
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<IActionResult> Index()
    {
        var menus = await _menuService.GetAllAsync();
        return View(menus.OrderByDescending(m => m.FechaInicio));
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var menu = await _menuService.GetByIdAsync(id);
        if (menu == null) return NotFound();
        return View(menu);
    }

    public async Task<IActionResult> Disponible()
    {
        var menu = await _menuService.GetDisponibleAsync();
        if (menu == null)
        {
            ViewBag.Mensaje = "No hay ningún menú disponible en este momento.";
            return View(null);
        }
        return View(menu);
    }

    public async Task<IActionResult> Mantenimiento()
    {
        var menus = await _menuService.GetAllForMantenimientoAsync();
        return View("Mantenimiento/Index", menus.OrderByDescending(m => m.FechaInicio));
    }

    public async Task<IActionResult> Crear()
    {
        var productos = await _menuService.GetProductosDisponiblesAsync();
        var combos = await _menuService.GetCombosDisponiblesAsync();
        ViewBag.Productos = productos;
        ViewBag.Combos = combos;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CreateMenuDTO dto)
    {
        if (ModelState.IsValid)
        {
            if (dto.FechaInicio.HasValue && dto.FechaFin.HasValue && dto.FechaInicio > dto.FechaFin)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin debe ser mayor o igual a la fecha de inicio");
                var prods = await _menuService.GetProductosDisponiblesAsync();
                var comb = await _menuService.GetCombosDisponiblesAsync();
                ViewBag.Productos = prods;
                ViewBag.Combos = comb;
                return View(dto);
            }

            if (dto.HoraInicio.HasValue && dto.HoraFin.HasValue && dto.HoraInicio >= dto.HoraFin)
            {
                ModelState.AddModelError("HoraFin", "La hora de fin debe ser estrictamente mayor que la hora de inicio. Los turnos que crucen medianoche (ej. 22:00-02:00) no están permitidos.");
                var prods = await _menuService.GetProductosDisponiblesAsync();
                var comb = await _menuService.GetCombosDisponiblesAsync();
                ViewBag.Productos = prods;
                ViewBag.Combos = comb;
                return View(dto);
            }

            await _menuService.CreateAsync(dto);
            return RedirectToAction(nameof(Mantenimiento));
        }

        var productos = await _menuService.GetProductosDisponiblesAsync();
        var combos = await _menuService.GetCombosDisponiblesAsync();
        ViewBag.Productos = productos;
        ViewBag.Combos = combos;
        return View(dto);
    }

    public async Task<IActionResult> Modificar(int id)
    {
        var menu = await _menuService.GetByIdForEditAsync(id);
        if (menu == null) return NotFound();

        var productos = await _menuService.GetAllProductosAsync();
        var combos = await _menuService.GetAllCombosAsync();
        ViewBag.Productos = productos;
        ViewBag.Combos = combos;

        return View(menu);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Modificar(UpdateMenuDTO dto)
    {
        if (ModelState.IsValid)
        {
            if (dto.FechaInicio.HasValue && dto.FechaFin.HasValue && dto.FechaInicio > dto.FechaFin)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin debe ser mayor o igual a la fecha de inicio");
                var prods = await _menuService.GetProductosDisponiblesAsync();
                var comb = await _menuService.GetCombosDisponiblesAsync();
                ViewBag.Productos = prods;
                ViewBag.Combos = comb;
                return View(dto);
            }

            if (dto.HoraInicio.HasValue && dto.HoraFin.HasValue && dto.HoraInicio >= dto.HoraFin)
            {
                ModelState.AddModelError("HoraFin", "La hora de fin debe ser estrictamente mayor que la hora de inicio. Los turnos que crucen medianoche (ej. 22:00-02:00) no están permitidos.");
                var prods = await _menuService.GetProductosDisponiblesAsync();
                var comb = await _menuService.GetCombosDisponiblesAsync();
                ViewBag.Productos = prods;
                ViewBag.Combos = comb;
                return View(dto);
            }

            await _menuService.UpdateAsync(dto);
            return RedirectToAction(nameof(Mantenimiento));
        }

        var productos = await _menuService.GetProductosDisponiblesAsync();
        var combos = await _menuService.GetCombosDisponiblesAsync();
        ViewBag.Productos = productos;
        ViewBag.Combos = combos;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _menuService.DeleteAsync(id);
        return RedirectToAction(nameof(Mantenimiento));
    }
}