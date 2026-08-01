namespace FastBurger.Application.DTOs;

public class ProcesoPreparacionDTO
{
    public int IdProceso { get; set; }
    public int IdProducto { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public string? NombreProducto { get; set; }
    public int CantidadPasos { get; set; }
    public List<PasoDTO> Pasos { get; set; } = new();
}

public class PasoDTO
{
    public int IdPaso { get; set; }
    public int Orden { get; set; }
    public string NombreEstacion { get; set; } = null!;
    public int IdEstacion { get; set; }
    public string? Descripcion { get; set; }
    public int TiempoMin { get; set; }
    public int? TemperaturaC { get; set; }
}

public class CreateProcesoPreparacionDTO
{
    public int IdProducto { get; set; }
    public string? Descripcion { get; set; }
    public List<PasoCreacionDTO> Pasos { get; set; } = new();
}

public class PasoCreacionDTO
{
    public int IdEstacion { get; set; }
    public int Orden { get; set; }
    public string Descripcion { get; set; } = null!;
    public int TiempoMin { get; set; }
    public int? TemperaturaC { get; set; }
}

public class UpdateProcesoPreparacionDTO
{
    public int IdProceso { get; set; }
    public int IdProducto { get; set; }
    public string? NombreProducto { get; set; }
    public string? Descripcion { get; set; }
    public List<PasoCreacionDTO> Pasos { get; set; } = new();
}

public class EstacionSimpleDTO
{
    public int IdEstacion { get; set; }
    public string Nombre { get; set; } = null!;
}