using FastBurger.Application.DTOs;

namespace FastBurger.Application.Interfaces;

public interface IProductoService
{
    Task<IEnumerable<ProductoDTO>> GetAllAsync();
    Task<IEnumerable<ProductoDTO>> GetAllForMantenimientoAsync();
    Task<ProductoDTO?> GetByIdAsync(int id);
    Task<ProductoDTO> CreateAsync(CreateProductoDTO dto);
    Task UpdateAsync(UpdateProductoDTO dto);
    Task<bool> ExisteNombreAsync(string nombre, int? idProductoExcluir = null);
    Task DeleteAsync(int id);
    Task<IEnumerable<CategoriaDTO>> GetCategoriasAsync();
    Task<IEnumerable<IngredienteDTO>> GetIngredientesAsync();
}