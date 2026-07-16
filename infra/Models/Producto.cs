using System;
using System.Collections.Generic;

namespace FastBurger.Infrastructure.Models;

public partial class Producto
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

    public virtual ICollection<CarritoItem> CarritoItems { get; set; } = new List<CarritoItem>();

    public virtual ICollection<ComboProducto> ComboProductos { get; set; } = new List<ComboProducto>();

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual CategoriaProducto IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<MenuProducto> MenuProductos { get; set; } = new List<MenuProducto>();

    public virtual ProcesoPreparacion? ProcesoPreparacion { get; set; }

    public virtual ICollection<ProductoIngrediente> ProductoIngredientes { get; set; } = new List<ProductoIngrediente>();
}
