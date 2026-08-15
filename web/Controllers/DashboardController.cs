    using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

[Authorize(Roles = "Administrador,Encargado")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var dto = await _dashboardService.ObtenerResumenHoyAsync();
        return View(dto);
    }
}