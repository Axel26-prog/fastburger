namespace FastBurger.Application.DTOs;

public class ProductoDTO
{
    public int IdProducto { get; set; }
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public bool Disponible { get; set; }
    public int TiempoPrepMin { get; set; }
    public int? Calorias { get; set; }
    public string? NombreCategoria { get; set; }
    public List<string> Ingredientes { get; set; } = new();
    public List<int> IngredienteIds { get; set; } = new();
}

public class CreateProductoDTO
{
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public bool Disponible { get; set; }
    public int TiempoPrepMin { get; set; }
    public int? Calorias { get; set; }
    public List<int> IngredienteIds { get; set; } = new();
}

public class UpdateProductoDTO
{
    public int IdProducto { get; set; }
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public string? ImagenUrlActual { get; set; }
    public bool Disponible { get; set; }
    public int TiempoPrepMin { get; set; }
    public int? Calorias { get; set; }
    public List<int> IngredienteIds { get; set; } = new();
}

public class CategoriaDTO
{
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activa { get; set; }
}

public class IngredienteDTO
{
    public int IdIngrediente { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Alergenico { get; set; }
    public string UnidadMedida { get; set; } = null!;
}