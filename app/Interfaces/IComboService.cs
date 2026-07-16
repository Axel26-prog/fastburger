using FastBurger.Application.DTOs;

namespace FastBurger.Application.Interfaces;

public interface IComboService
{
    Task<IEnumerable<ComboDTO>> GetAllAsync();
    Task<ComboDTO?> GetByIdAsync(int id);
    Task<ComboDTO> CreateAsync(CreateComboDTO dto);
    Task UpdateAsync(UpdateComboDTO dto);
    Task DeleteAsync(int id);
}