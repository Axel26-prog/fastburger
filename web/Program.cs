using FastBurger.Application.Interfaces;
using FastBurger.Application.Services;
using FastBurger.Infrastructure.Data;
using FastBurger.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FastBurgerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IComboService, ComboService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IProcesoPreparacionService, ProcesoPreparacionService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();
builder.Services.AddScoped<ISesionUsuarioService, SesionUsuarioService>();
builder.Services.AddScoped<IOrdenCocinaService, OrdenCocinaService>();
builder.Services.AddScoped<IAutenticacionService, AutenticacionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddHostedService<CarritoLimpiezaBackgroundService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());

    var mb = options.ModelBindingMessageProvider;
    mb.SetAttemptedValueIsInvalidAccessor((value, field) => "El valor ingresado no es válido.");
    mb.SetMissingBindRequiredValueAccessor(field => "Este campo es obligatorio.");
    mb.SetMissingKeyOrValueAccessor(() => "Este campo es obligatorio.");
    mb.SetMissingRequestBodyRequiredValueAccessor(() => "Se requiere un cuerpo de solicitud no vacío.");
    mb.SetNonPropertyAttemptedValueIsInvalidAccessor(value => "El valor ingresado no es válido.");
    mb.SetNonPropertyUnknownValueIsInvalidAccessor(() => "El valor proporcionado no es válido.");
    mb.SetNonPropertyValueMustBeANumberAccessor(() => "El campo debe ser un número.");
    mb.SetUnknownValueIsInvalidAccessor(field => "El valor proporcionado no es válido.");
    mb.SetValueIsInvalidAccessor(value => $"El valor '{value}' no es válido.");
    mb.SetValueMustBeANumberAccessor(field => "El campo debe ser un número.");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.LogoutPath = "/Cuenta/Logout";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToAccessDenied = context =>
            {
                var roles = "";
                var endpoint = context.HttpContext.GetEndpoint();
                if (endpoint?.Metadata != null)
                {
                    var authz = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
                    if (authz != null && !string.IsNullOrEmpty(authz.Roles))
                        roles = authz.Roles;
                }

                var uri = context.RedirectUri;
                if (!string.IsNullOrEmpty(roles))
                    uri += (uri.Contains('?') ? "&" : "?") + "rol=" + Uri.EscapeDataString(roles);

                context.Response.Redirect(uri);
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FastBurgerContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();
    await HashearUsuariosExistentes(context, hasher);
}

app.Run();

static async Task HashearUsuariosExistentes(FastBurgerContext context, IPasswordHasher<Usuario> hasher)
{
    var usuarios = await context.Usuarios
        .Where(u => !u.Contrasena.StartsWith("AQAAAA"))
        .ToListAsync();

    if (usuarios.Count == 0)
        return;

    foreach (var u in usuarios)
        u.Contrasena = hasher.HashPassword(u, u.Contrasena);

    await context.SaveChangesAsync();
}