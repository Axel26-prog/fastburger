using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class MenuCombo
{
    public int IdMenu { get; set; }

    public int IdCombo { get; set; }

    public decimal? PrecioEspecial { get; set; }

    public virtual Combo IdComboNavigation { get; set; } = null!;

    public virtual Menu IdMenuNavigation { get; set; } = null!;
}
