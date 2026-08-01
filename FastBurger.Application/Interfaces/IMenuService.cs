using FastBurger.Application.DTOs;

namespace FastBurger.Application.Interfaces;

public interface IMenuService
{
    Task<IEnumerable<MenuDTO>> GetAllAsync();
    Task<IEnumerable<MenuDTO>> GetAllForMantenimientoAsync();
    Task<MenuDTO?> GetByIdAsync(int id);
    Task<MenuForEditDTO?> GetByIdForEditAsync(int id);
    Task<MenuDTO?> GetDisponibleAsync();
    Task<MenuDTO> CreateAsync(CreateMenuDTO dto);
    Task UpdateAsync(UpdateMenuDTO dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<ProductoSimpleDTO>> GetProductosDisponiblesAsync();
    Task<IEnumerable<ComboSimpleDTO>> GetCombosDisponiblesAsync();
    Task<IEnumerable<ProductoSimpleDTO>> GetAllProductosAsync();
    Task<IEnumerable<ComboSimpleDTO>> GetAllCombosAsync();
}