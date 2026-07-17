using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

public class ProcesoPreparacionController : Controller
{
    private readonly IProcesoPreparacionService _procesoPreparacionService;

    public ProcesoPreparacionController(IProcesoPreparacionService procesoPreparacionService)
    {
        _procesoPreparacionService = procesoPreparacionService;
    }

    public async Task<IActionResult> Index()
    {
        var procesos = await _procesoPreparacionService.GetAllAsync();
        return View(procesos);
    }

    public async Task<IActionResult> Mantenimiento()
    {
        var procesos = await _procesoPreparacionService.GetAllAsync();
        return View("Mantenimiento/Index", procesos);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var proceso = await _procesoPreparacionService.GetByIdAsync(id);
        if (proceso == null) return NotFound();
        return View(proceso);
    }

    public async Task<IActionResult> Crear()
    {
        var productos = await _procesoPreparacionService.GetProductosDisponiblesAsync();
        var estaciones = await _procesoPreparacionService.GetEstacionesAsync();
        ViewBag.Productos = productos;
        ViewBag.Estaciones = estaciones;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CreateProcesoPreparacionDTO dto)
    {
        if (ModelState.IsValid)
        {
            await _procesoPreparacionService.CreateAsync(dto);
            return RedirectToAction(nameof(Mantenimiento));
        }

        var productos = await _procesoPreparacionService.GetProductosDisponiblesAsync();
        var estaciones = await _procesoPreparacionService.GetEstacionesAsync();
        ViewBag.Productos = productos;
        ViewBag.Estaciones = estaciones;
        return View(dto);
    }

    public async Task<IActionResult> Modificar(int id)
    {
        var proceso = await _procesoPreparacionService.GetByIdAsync(id);
        if (proceso == null) return NotFound();

        var procesoVM = new UpdateProcesoPreparacionDTO
        {
            IdProceso = proceso.IdProceso,
            IdProducto = proceso.IdProducto,
            Descripcion = proceso.Descripcion,
            Pasos = proceso.Pasos.Select(p => new PasoCreacionDTO
            {
                IdEstacion = p.IdEstacion,
                Orden = p.Orden,
                Descripcion = p.Descripcion ?? string.Empty,
                TiempoMin = p.TiempoMin,
                TemperaturaC = p.TemperaturaC
            }).ToList()
        };

        var productos = await _procesoPreparacionService.GetProductosDisponiblesAsync();
        var estaciones = await _procesoPreparacionService.GetEstacionesAsync();
        ViewBag.Productos = productos;
        ViewBag.Estaciones = estaciones;

        return View(procesoVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Modificar(UpdateProcesoPreparacionDTO dto)
    {
        if (ModelState.IsValid)
        {
            await _procesoPreparacionService.UpdateAsync(dto);
            return RedirectToAction(nameof(Mantenimiento));
        }

        var productos = await _procesoPreparacionService.GetProductosDisponiblesAsync();
        var estaciones = await _procesoPreparacionService.GetEstacionesAsync();
        ViewBag.Productos = productos;
        ViewBag.Estaciones = estaciones;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _procesoPreparacionService.DeleteAsync(id);
        return RedirectToAction(nameof(Mantenimiento));
    }
}