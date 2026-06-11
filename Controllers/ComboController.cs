using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

public class ComboController : Controller
{
    private readonly IComboService _comboService;

    public ComboController(IComboService comboService)
    {
        _comboService = comboService;
    }

    public async Task<IActionResult> Index()
    {
        var combos = await _comboService.GetAllAsync();
        return View(combos);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var combo = await _comboService.GetByIdAsync(id);
        if (combo == null) return NotFound();
        return View(combo);
    }
}