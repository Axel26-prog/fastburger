using FastBurger.Application.DTOs;

namespace FastBurger.Application.Interfaces;

public interface IMenuService
{
    Task<IEnumerable<MenuDTO>> GetAllAsync();
    Task<MenuDTO?> GetByIdAsync(int id);
    Task<MenuDTO?> GetDisponibleAsync();
    Task<MenuDTO> CreateAsync(CreateMenuDTO dto);
    Task UpdateAsync(UpdateMenuDTO dto);
    Task DeleteAsync(int id);
}