using System;
using System.Collections.Generic;
using FastBurger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FastBurger.Infrastructure.Data;


public partial class FastBurgerContext : DbContext
{
    public FastBurgerContext()
    {
    }

    public FastBurgerContext(DbContextOptions<FastBurgerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Carrito> Carritos { get; set; }

    public virtual DbSet<CarritoItem> CarritoItems { get; set; }

    public virtual DbSet<CategoriaProducto> CategoriaProductos { get; set; }

    public virtual DbSet<Combo> Combos { get; set; }

    public virtual DbSet<ComboProducto> ComboProductos { get; set; }

    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    public virtual DbSet<DireccionUsuario> DireccionUsuarios { get; set; }

    public virtual DbSet<EstacionCocina> EstacionCocinas { get; set; }

    public virtual DbSet<Ingrediente> Ingredientes { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenuCombo> MenuCombos { get; set; }

    public virtual DbSet<MenuProducto> MenuProductos { get; set; }

    public virtual DbSet<MetodoPago> MetodoPagos { get; set; }

    public virtual DbSet<OrdenCocina> OrdenCocinas { get; set; }

    public virtual DbSet<OrdenCocinaItem> OrdenCocinaItems { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<PasoPreparacion> PasoPreparacions { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<ProcesoPreparacion> ProcesoPreparacions { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<ProductoIngrediente> ProductoIngredientes { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-A045J8P\\MSSQLSERVERR;Database=fastburger;User Id=sa;Password=123456;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Modern_Spanish_CI_AI");

        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.IdCarrito).HasName("PK__carrito__83A2AD9C838CD9FB");

            entity.ToTable("carrito", tb => tb.HasTrigger("trg_carrito_actualizacion"));

            entity.Property(e => e.IdCarrito).HasColumnName("id_carrito");
            entity.Property(e => e.Estado)
                .HasMaxLength(15)
                .HasDefaultValue("activo")
                .HasColumnName("estado");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Carritos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_carrito_usuario");
        });

        modelBuilder.Entity<CarritoItem>(entity =>
        {
            entity.HasKey(e => e.IdItem).HasName("PK__carrito___87C9438B2E02DA89");

            entity.ToTable("carrito_item");

            entity.Property(e => e.IdItem).HasColumnName("id_item");
            entity.Property(e => e.Cantidad)
                .HasDefaultValue((short)1)
                .HasColumnName("cantidad");
            entity.Property(e => e.IdCarrito).HasColumnName("id_carrito");
            entity.Property(e => e.IdCombo).HasColumnName("id_combo");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Notas)
                .HasMaxLength(300)
                .HasColumnName("notas");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("precio_unitario");

            entity.HasOne(d => d.IdCarritoNavigation).WithMany(p => p.CarritoItems)
                .HasForeignKey(d => d.IdCarrito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ci_carrito");

            entity.HasOne(d => d.IdComboNavigation).WithMany(p => p.CarritoItems)
                .HasForeignKey(d => d.IdCombo)
                .HasConstraintName("fk_ci_combo");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.CarritoItems)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("fk_ci_producto");
        });

        modelBuilder.Entity<CategoriaProducto>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__categori__CD54BC5A19A05FD4");

            entity.ToTable("categoria_producto");

            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Activa)
                .HasDefaultValue(true)
                .HasColumnName("activa");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(300)
                .HasColumnName("imagen_url");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.IdCombo).HasName("PK__combo__7F0902ED686B0BB7");

            entity.ToTable("combo");

            entity.Property(e => e.IdCombo).HasColumnName("id_combo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.Disponible)
                .HasDefaultValue(true)
                .HasColumnName("disponible");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(300)
                .HasColumnName("imagen_url");
            entity.Property(e => e.Nombre)
                .HasMaxLength(120)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("precio");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Combos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_combo_categoria");
        });

        modelBuilder.Entity<ComboProducto>(entity =>
        {
            entity.HasKey(e => new { e.IdCombo, e.IdProducto }).HasName("pk_combo_producto");

            entity.ToTable("combo_producto");

            entity.Property(e => e.IdCombo).HasColumnName("id_combo");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Cantidad)
                .HasDefaultValue((short)1)
                .HasColumnName("cantidad");

            entity.HasOne(d => d.IdComboNavigation).WithMany(p => p.ComboProductos)
                .HasForeignKey(d => d.IdCombo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cp_combo");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ComboProductos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cp_producto");
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__detalle___4F1332DE43A17561");

            entity.ToTable("detalle_pedido");

            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.Cantidad)
                .HasDefaultValue((short)1)
                .HasColumnName("cantidad");
            entity.Property(e => e.IdCombo).HasColumnName("id_combo");
            entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Notas)
                .HasMaxLength(300)
                .HasColumnName("notas");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("precio_unitario");

            entity.HasOne(d => d.IdComboNavigation).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.IdCombo)
                .HasConstraintName("fk_dp_combo");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dp_pedido");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("fk_dp_producto");
        });

        modelBuilder.Entity<DireccionUsuario>(entity =>
        {
            entity.HasKey(e => e.IdDireccion).HasName("PK__direccio__25C35D07B41E9048");

            entity.ToTable("direccion_usuario");

            entity.Property(e => e.IdDireccion).HasColumnName("id_direccion");
            entity.Property(e => e.Alias)
                .HasMaxLength(80)
                .HasColumnName("alias");
            entity.Property(e => e.Canton)
                .HasMaxLength(80)
                .HasColumnName("canton");
            entity.Property(e => e.DireccionExacta)
                .HasMaxLength(300)
                .HasColumnName("direccion_exacta");
            entity.Property(e => e.Distrito)
                .HasMaxLength(80)
                .HasColumnName("distrito");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Predeterminada).HasColumnName("predeterminada");
            entity.Property(e => e.Provincia)
                .HasMaxLength(80)
                .HasColumnName("provincia");
            entity.Property(e => e.Referencia)
                .HasMaxLength(200)
                .HasColumnName("referencia");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.DireccionUsuarios)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dir_usuario");
        });

        modelBuilder.Entity<EstacionCocina>(entity =>
        {
            entity.HasKey(e => e.IdEstacion).HasName("PK__estacion__1F3B45EBDED03BAD");

            entity.ToTable("estacion_cocina");

            entity.Property(e => e.IdEstacion).HasColumnName("id_estacion");
            entity.Property(e => e.Activa)
                .HasDefaultValue(true)
                .HasColumnName("activa");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.IdIngrediente).HasName("PK__ingredie__3F505D45F8DFEBCB");

            entity.ToTable("ingrediente");

            entity.Property(e => e.IdIngrediente).HasColumnName("id_ingrediente");
            entity.Property(e => e.Alergenico).HasColumnName("alergenico");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(120)
                .HasColumnName("nombre");
            entity.Property(e => e.UnidadMedida)
                .HasMaxLength(30)
                .HasColumnName("unidad_medida");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdMenu).HasName("PK__menu__68A1D9DB8032CD11");

            entity.ToTable("menu");

            entity.Property(e => e.IdMenu).HasColumnName("id_menu");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .HasColumnName("descripcion");
            entity.Property(e => e.DiasSemana)
                .HasMaxLength(20)
                .HasColumnName("dias_semana");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.HoraFin).HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio).HasColumnName("hora_inicio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<MenuCombo>(entity =>
        {
            entity.HasKey(e => new { e.IdMenu, e.IdCombo }).HasName("pk_menu_combo");

            entity.ToTable("menu_combo");

            entity.Property(e => e.IdMenu).HasColumnName("id_menu");
            entity.Property(e => e.IdCombo).HasColumnName("id_combo");
            entity.Property(e => e.PrecioEspecial)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("precio_especial");

            entity.HasOne(d => d.IdComboNavigation).WithMany(p => p.MenuCombos)
                .HasForeignKey(d => d.IdCombo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mc_combo");

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.MenuCombos)
                .HasForeignKey(d => d.IdMenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mc_menu");
        });

        modelBuilder.Entity<MenuProducto>(entity =>
        {
            entity.HasKey(e => new { e.IdMenu, e.IdProducto }).HasName("pk_menu_producto");

            entity.ToTable("menu_producto");

            entity.Property(e => e.IdMenu).HasColumnName("id_menu");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.PrecioEspecial)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("precio_especial");

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.MenuProductos)
                .HasForeignKey(d => d.IdMenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mp_menu");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.MenuProductos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mp_producto");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodo).HasName("PK__metodo_p__1BBFF0F4EB7A1CA5");

            entity.ToTable("metodo_pago");

            entity.Property(e => e.IdMetodo).HasColumnName("id_metodo");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<OrdenCocina>(entity =>
        {
            entity.HasKey(e => e.IdOrdenCocina).HasName("PK__orden_co__0FC1E278A6540331");

            entity.ToTable("orden_cocina");

            entity.Property(e => e.IdOrdenCocina).HasColumnName("id_orden_cocina");
            entity.Property(e => e.Estado)
                .HasMaxLength(15)
                .HasDefaultValue("en_espera")
                .HasColumnName("estado");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaIngreso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_ingreso");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.IdEstacion).HasColumnName("id_estacion");
            entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
            entity.Property(e => e.NotasCocina)
                .HasMaxLength(300)
                .HasColumnName("notas_cocina");
            entity.Property(e => e.Prioridad)
                .HasDefaultValue((short)5)
                .HasColumnName("prioridad");

            entity.HasOne(d => d.IdEstacionNavigation).WithMany(p => p.OrdenCocinas)
                .HasForeignKey(d => d.IdEstacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_oc_estacion");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.OrdenCocinas)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_oc_pedido");
        });

        modelBuilder.Entity<OrdenCocinaItem>(entity =>
        {
            entity.HasKey(e => new { e.IdOrdenCocina, e.IdDetalle }).HasName("pk_orden_cocina_item");

            entity.ToTable("orden_cocina_item");

            entity.Property(e => e.IdOrdenCocina).HasColumnName("id_orden_cocina");
            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.EstadoItem)
                .HasMaxLength(15)
                .HasDefaultValue("pendiente")
                .HasColumnName("estado_item");

            entity.HasOne(d => d.IdDetalleNavigation).WithMany(p => p.OrdenCocinaItems)
                .HasForeignKey(d => d.IdDetalle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_oci_detalle");

            entity.HasOne(d => d.IdOrdenCocinaNavigation).WithMany(p => p.OrdenCocinaItems)
                .HasForeignKey(d => d.IdOrdenCocina)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_oci_orden");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__pago__0941B074D7390CF0");

            entity.ToTable("pago");

            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.Estado)
                .HasMaxLength(15)
                .HasDefaultValue("pendiente")
                .HasColumnName("estado");
            entity.Property(e => e.FechaPago)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_pago");
            entity.Property(e => e.IdMetodo).HasColumnName("id_metodo");
            entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.MontoRecibido)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_recibido");
            entity.Property(e => e.Referencia)
                .HasMaxLength(200)
                .HasColumnName("referencia");

            entity.HasOne(d => d.IdMetodoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdMetodo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pago_metodo");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pago_pedido");
        });

        modelBuilder.Entity<PasoPreparacion>(entity =>
        {
            entity.HasKey(e => e.IdPaso).HasName("PK__paso_pre__094112E7D36AA6CD");

            entity.ToTable("paso_preparacion");

            entity.HasIndex(e => new { e.IdProceso, e.Orden }, "uq_paso_orden").IsUnique();

            entity.Property(e => e.IdPaso).HasColumnName("id_paso");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdEstacion).HasColumnName("id_estacion");
            entity.Property(e => e.IdProceso).HasColumnName("id_proceso");
            entity.Property(e => e.Orden).HasColumnName("orden");
            entity.Property(e => e.TemperaturaC).HasColumnName("temperatura_c");
            entity.Property(e => e.TiempoMin).HasColumnName("tiempo_min");

            entity.HasOne(d => d.IdEstacionNavigation).WithMany(p => p.PasoPreparacions)
                .HasForeignKey(d => d.IdEstacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_paso_estacion");

            entity.HasOne(d => d.IdProcesoNavigation).WithMany(p => p.PasoPreparacions)
                .HasForeignKey(d => d.IdProceso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_paso_proceso");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.IdPedido).HasName("PK__pedido__6FF0148979397392");

            entity.ToTable("pedido");

            entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
            entity.Property(e => e.CostoEnvio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("costo_envio");
            entity.Property(e => e.Descuento)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("descuento");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("pendiente_pago")
                .HasColumnName("estado");
            entity.Property(e => e.FechaEntregaEstimada).HasColumnName("fecha_entrega_estimada");
            entity.Property(e => e.FechaEntregaReal).HasColumnName("fecha_entrega_real");
            entity.Property(e => e.FechaPedido)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_pedido");
            entity.Property(e => e.IdCarrito).HasColumnName("id_carrito");
            entity.Property(e => e.IdDireccion).HasColumnName("id_direccion");
            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Impuesto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("impuesto");
            entity.Property(e => e.Notas)
                .HasMaxLength(500)
                .HasColumnName("notas");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subtotal");
            entity.Property(e => e.TipoEntrega)
                .HasMaxLength(15)
                .HasDefaultValue("recoger")
                .HasColumnName("tipo_entrega");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");

            entity.HasOne(d => d.IdCarritoNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdCarrito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pedido_carrito");

            entity.HasOne(d => d.IdDireccionNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdDireccion)
                .HasConstraintName("fk_pedido_direccion");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.PedidoIdEmpleadoNavigations)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("fk_pedido_empleado");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.PedidoIdUsuarioNavigations)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pedido_usuario");
        });

        modelBuilder.Entity<ProcesoPreparacion>(entity =>
        {
            entity.HasKey(e => e.IdProceso).HasName("PK__proceso___4D1766E4E70A59D5");

            entity.ToTable("proceso_preparacion");

            entity.HasIndex(e => e.IdProducto, "uq_pp_producto").IsUnique();

            entity.Property(e => e.IdProceso).HasColumnName("id_proceso");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");

            entity.HasOne(d => d.IdProductoNavigation).WithOne(p => p.ProcesoPreparacion)
                .HasForeignKey<ProcesoPreparacion>(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pp_producto");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__producto__FF341C0D24FEBECB");

            entity.ToTable("producto");

            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Calorias).HasColumnName("calorias");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.Disponible)
                .HasDefaultValue(true)
                .HasColumnName("disponible");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(300)
                .HasColumnName("imagen_url");
            entity.Property(e => e.Nombre)
                .HasMaxLength(120)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.TiempoPrepMin)
                .HasDefaultValue(5)
                .HasColumnName("tiempo_prep_min");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_prod_categoria");
        });

        modelBuilder.Entity<ProductoIngrediente>(entity =>
        {
            entity.HasKey(e => new { e.IdProducto, e.IdIngrediente }).HasName("pk_producto_ingrediente");

            entity.ToTable("producto_ingrediente");

            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdIngrediente).HasColumnName("id_ingrediente");
            entity.Property(e => e.Cantidad)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("cantidad");
            entity.Property(e => e.Opcional).HasColumnName("opcional");

            entity.HasOne(d => d.IdIngredienteNavigation).WithMany(p => p.ProductoIngredientes)
                .HasForeignKey(d => d.IdIngrediente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pi_ingrediente");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductoIngredientes)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pi_producto");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__rol__6ABCB5E04E5CB9B1");

            entity.ToTable("rol");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuario__4E3E04AD70236FA0");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Correo, "uq_usuario_correo").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .HasColumnName("apellidos");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255)
                .HasColumnName("contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.FotoPerfil)
                .HasMaxLength(300)
                .HasColumnName("foto_perfil");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
            entity.Property(e => e.TokenReset)
                .HasMaxLength(100)
                .HasColumnName("token_reset");
            entity.Property(e => e.TokenResetExpira).HasColumnName("token_reset_expira");
            entity.Property(e => e.UltimoAcceso).HasColumnName("ultimo_acceso");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuario_rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
