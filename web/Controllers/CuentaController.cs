using System.Security.Claims;
using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

[AllowAnonymous]
public class CuentaController : Controller
{
    private readonly IAutenticacionService _autenticacionService;

    public CuentaController(IAutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDTO dto, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
            return View(dto);

        var resultado = await _autenticacionService.LoginAsync(dto.Correo, dto.Contrasena);
        if (!resultado.Exito)
        {
            ModelState.AddModelError("", resultado.Error ?? "Correo o contraseña incorrectos");
            return View(dto);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, resultado.IdUsuario.ToString()),
            new(ClaimTypes.Name, resultado.Nombre + " " + resultado.Apellidos),
            new(ClaimTypes.Email, resultado.Correo ?? ""),
            new("Nombre", resultado.Nombre ?? ""),
            new("Apellidos", resultado.Apellidos ?? ""),
            new(ClaimTypes.Role, resultado.NombreRol ?? "")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var propiedades = new AuthenticationProperties
        {
            IsPersistent = dto.Recordarme,
            ExpiresUtc = dto.Recordarme ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddMinutes(30)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, propiedades);

        TempData["Success"] = "Bienvenido, " + resultado.Nombre;

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Registro()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroClienteDTO dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _autenticacionService.RegistrarClienteAsync(dto);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }

        TempData["Success"] = "Cuenta creada, ya puede iniciar sesión";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Success"] = "Sesión cerrada";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccesoDenegado(string? rol = null)
    {
        ViewBag.RolRequerido = rol;
        return View();
    }
}