using FastBurger.Application.DTOs;

namespace FastBurger.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDTO> ObtenerResumenHoyAsync();
}