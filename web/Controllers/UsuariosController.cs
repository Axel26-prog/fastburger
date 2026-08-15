using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastBurger.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class UsuariosController : Controller
{
    private readonly IAutenticacionService _autenticacionService;

    public UsuariosController(IAutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _autenticacionService.ListarUsuariosAsync();
        return View(usuarios);
    }

    public IActionResult Crear()
    {
        ViewBag.Roles = new List<string> { "Encargado", "Cocina" };
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearStaffDTO dto)
    {
        ViewBag.Roles = new List<string> { "Encargado", "Cocina" };

        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            var usuario = await _autenticacionService.CrearStaffAsync(dto);
            TempData["Success"] = $"Usuario {usuario.Nombre} {usuario.Apellidos} creado como {dto.Rol}";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activar(int id)
    {
        try
        {
            await _autenticacionService.CambiarEstadoUsuarioAsync(id, true);
            TempData["Success"] = "Usuario activado";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desactivar(int id)
    {
        try
        {
            await _autenticacionService.CambiarEstadoUsuarioAsync(id, false);
            TempData["Success"] = "Usuario desactivado";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ResetPassword(int id)
    {
        var usuario = await _autenticacionService.ObtenerUsuarioAsync(id);
        if (usuario == null)
        {
            TempData["Error"] = "Usuario no encontrado";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Usuario = usuario;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(int id, string nuevaContrasena, string confirmarContrasena)
    {
        var usuario = await _autenticacionService.ObtenerUsuarioAsync(id);
        ViewBag.Usuario = usuario;

        if (usuario == null)
        {
            TempData["Error"] = "Usuario no encontrado";
            return RedirectToAction(nameof(Index));
        }

        if (nuevaContrasena != confirmarContrasena)
        {
            ModelState.AddModelError("", "Las contraseñas no coinciden");
            return View();
        }

        if (string.IsNullOrWhiteSpace(nuevaContrasena) || nuevaContrasena.Length < 6)
        {
            ModelState.AddModelError("", "Mínimo 6 caracteres");
            return View();
        }

        try
        {
            await _autenticacionService.ResetPasswordAsync(id, nuevaContrasena);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }

        TempData["Success"] = $"Contraseña de {usuario.NombreCompleto} actualizada";
        return RedirectToAction(nameof(Index));
    }
}