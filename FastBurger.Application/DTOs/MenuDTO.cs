namespace FastBurger.Application.DTOs;

public class ProductoSimpleDTO
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
}

public class ComboSimpleDTO
{
    public int IdCombo { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
}

public class MenuDTO
{
    public int IdMenu { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public TimeOnly? HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public string? DiasSemana { get; set; }
    public List<MenuItemDTO> Items { get; set; } = new();
}

public class MenuItemDTO
{
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public string Categoria { get; set; } = null!;
    public string Tipo { get; set; } = null!;
}

public class CreateMenuDTO
{
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public TimeOnly? HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public string? DiasSemana { get; set; }
    public List<int> ProductoIds { get; set; } = new();
    public List<int> ComboIds { get; set; } = new();
    public Dictionary<int, decimal?> PreciosProductos { get; set; } = new();
    public Dictionary<int, decimal?> PreciosCombos { get; set; } = new();
}

public class UpdateMenuDTO
{
    public int IdMenu { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public TimeOnly? HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public string? DiasSemana { get; set; }
    public List<int> ProductoIds { get; set; } = new();
    public List<int> ComboIds { get; set; } = new();
    public Dictionary<int, decimal?> PreciosProductos { get; set; } = new();
    public Dictionary<int, decimal?> PreciosCombos { get; set; } = new();
}

public class MenuForEditDTO
{
    public int IdMenu { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public TimeOnly? HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public string? DiasSemana { get; set; }
    public List<int> ProductoIds { get; set; } = new();
    public List<int> ComboIds { get; set; } = new();
    public Dictionary<int, decimal?> PreciosProductos { get; set; } = new();
    public Dictionary<int, decimal?> PreciosCombos { get; set; } = new();
}