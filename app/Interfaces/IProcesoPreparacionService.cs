using FastBurger.Application.DTOs;

namespace FastBurger.Application.Interfaces;

public interface IProcesoPreparacionService
{
    Task<IEnumerable<ProcesoPreparacionDTO>> GetAllAsync();
    Task<ProcesoPreparacionDTO?> GetByIdAsync(int id);
    Task<ProcesoPreparacionDTO> CreateAsync(CreateProcesoPreparacionDTO dto);
    Task UpdateAsync(UpdateProcesoPreparacionDTO dto);
    Task DeleteAsync(int id);
}