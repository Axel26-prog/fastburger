namespace FastBurger.Application.DTOs;

public class ComboDTO
{
    public int IdCombo { get; set; }
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public bool Disponible { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public string? NombreCategoria { get; set; }
    public List<string> Productos { get; set; } = new();
    public List<int> ProductoIds { get; set; } = new();
}

public class CreateComboDTO
{
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public bool Disponible { get; set; }
    public string? ImagenUrl { get; set; }
    public List<int> ProductoIds { get; set; } = new();
}

public class UpdateComboDTO
{
    public int IdCombo { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public string? ImagenUrlActual { get; set; }
    public bool Disponible { get; set; }
    public List<int> ProductoIds { get; set; } = new();
}