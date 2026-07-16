using FastBurger.Application.DTOs;

namespace FastBurger.Application.Interfaces;

public interface IProductoService
{
    Task<IEnumerable<ProductoDTO>> GetAllAsync();
    Task<ProductoDTO?> GetByIdAsync(int id);
    Task<ProductoDTO> CreateAsync(CreateProductoDTO dto);
    Task UpdateAsync(UpdateProductoDTO dto);
    Task DeleteAsync(int id);
}