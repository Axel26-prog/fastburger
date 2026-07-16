namespace FastBurger.Application.DTOs;

public class ProcesoPreparacionDTO
{
    public int IdProceso { get; set; }
    public int IdProducto { get; set; }
    public string? Descripcion { get; set; }
    public string? NombreProducto { get; set; }
    public int CantidadPasos { get; set; }
    public List<PasoDTO> Pasos { get; set; } = new();
}

public class PasoDTO
{
    public int Orden { get; set; }
    public string NombreEstacion { get; set; } = null!;
}

public class CreateProcesoPreparacionDTO
{
    public int IdProducto { get; set; }
    public string? Descripcion { get; set; }
}

public class UpdateProcesoPreparacionDTO
{
    public int IdProceso { get; set; }
    public int IdProducto { get; set; }
    public string? Descripcion { get; set; }
}