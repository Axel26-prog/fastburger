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

    public async Task<IActionResult> Detalle(int id)
    {
        var proceso = await _procesoPreparacionService.GetByIdAsync(id);
        if (proceso == null) return NotFound();
        return View(proceso);
    }
}