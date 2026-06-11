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
        return View(menus);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var menu = await _menuService.GetByIdAsync(id);
        if (menu == null) return NotFound();
        return View(menu);
    }
}