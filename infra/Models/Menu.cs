using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class Menu
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

    public virtual ICollection<MenuCombo> MenuCombos { get; set; } = new List<MenuCombo>();

    public virtual ICollection<MenuProducto> MenuProductos { get; set; } = new List<MenuProducto>();
}
