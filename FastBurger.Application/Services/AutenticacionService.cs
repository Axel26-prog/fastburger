using FastBurger.Application.DTOs;
using FastBurger.Application.Interfaces;
using FastBurger.Infrastructure.Data;
using FastBurger.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastBurger.Application.Services;

public class AutenticacionService : IAutenticacionService
{
    private readonly FastBurgerContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public AutenticacionService(FastBurgerContext context, IPasswordHasher<Usuario> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<ResultadoLoginDTO> LoginAsync(string correo, string contrasena)
    {
        var resultado = new ResultadoLoginDTO();

        if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
        {
            resultado.Error = "Correo o contraseña incorrectos";
            return resultado;
        }

        var usuario = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u => u.Correo == correo);

        if (usuario == null)
        {
            resultado.Error = "Correo o contraseña incorrectos";
            return resultado;
        }

        if (!usuario.Activo)
        {
            resultado.Error = "Su cuenta está inactiva, contacte al administrador";
            return resultado;
        }

        
        if (usuario.IdRolNavigation == null || !usuario.IdRolNavigation.Activo)
        {
            resultado.Error = "El rol asignado no está disponible";
            return resultado;
        }

        if (_passwordHasher.VerifyHashedPassword(usuario, usuario.Contrasena, contrasena) == PasswordVerificationResult.Failed)
        {
            resultado.Error = "Correo o contraseña incorrectos";
            return resultado;
        }

        usuario.UltimoAcceso = DateTime.Now;
        await _context.SaveChangesAsync();

        resultado.Exito = true;
        resultado.IdUsuario = usuario.IdUsuario;
        resultado.Nombre = usuario.Nombre;
        resultado.Apellidos = usuario.Apellidos;
        resultado.Correo = usuario.Correo;
        resultado.IdRol = usuario.IdRol;
        resultado.NombreRol = usuario.IdRolNavigation.Nombre;

        return resultado;
    }

    public async Task<Usuario> RegistrarClienteAsync(RegistroClienteDTO dto)
    {
        var rolCliente = await _context.Rols.FirstOrDefaultAsync(r => r.IdRol == 4);
        if (rolCliente == null)
            throw new InvalidOperationException("El rol de Cliente no está configurado");

        if (!rolCliente.Activo)
            throw new InvalidOperationException("El rol de Cliente no está disponible");

        var correoExiste = await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo);
        if (correoExiste)
            throw new InvalidOperationException("Ese correo ya está registrado");

        var usuario = new Usuario
        {
            IdRol = 4,
            Nombre = dto.Nombre.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            Correo = dto.Correo.Trim(),
            Telefono = dto.Telefono?.Trim(),
            Activo = true,
            FechaRegistro = DateTime.Now
        };
        usuario.Contrasena = _passwordHasher.HashPassword(usuario, dto.Contrasena);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return usuario;
    }

    public async Task<Usuario> CrearStaffAsync(CrearStaffDTO dto)
    {
        int idRol;
        if (dto.Rol.ToLowerInvariant() == "encargado")
            idRol = 2;
        else if (dto.Rol.ToLowerInvariant() == "cocina")
            idRol = 3;
        else
            throw new InvalidOperationException("Solo se pueden crear usuarios con rol Encargado o Cocina");

        var rol = await _context.Rols.FirstOrDefaultAsync(r => r.IdRol == idRol);
        if (rol == null || !rol.Activo)
            throw new InvalidOperationException("El rol seleccionado no está disponible");

        if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
            throw new InvalidOperationException("Ese correo ya está registrado");

        var usuario = new Usuario
        {
            IdRol = idRol,
            Nombre = dto.Nombre.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            Correo = dto.Correo.Trim(),
            Telefono = dto.Telefono?.Trim(),
            Activo = true,
            FechaRegistro = DateTime.Now
        };
        usuario.Contrasena = _passwordHasher.HashPassword(usuario, dto.Contrasena);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return usuario;
    }

    public async Task<List<UsuarioListaDTO>> ListarUsuariosAsync()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .OrderBy(u => u.IdRol)
            .ThenBy(u => u.Nombre)
            .ToListAsync();

        return usuarios.Select(MapearUsuario).ToList();
    }

    public async Task<List<UsuarioListaDTO>> ListarClientesAsync()
    {
        var clientes = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Where(u => u.IdRol == 4)
            .OrderBy(u => u.Nombre)
            .ToListAsync();

        return clientes.Select(MapearUsuario).ToList();
    }

    public async Task<UsuarioListaDTO?> ObtenerUsuarioAsync(int id)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

        return usuario == null ? null : MapearUsuario(usuario);
    }

    private static UsuarioListaDTO MapearUsuario(Usuario u)
    {
        return new UsuarioListaDTO
        {
            IdUsuario = u.IdUsuario,
            Nombre = u.Nombre,
            Apellidos = u.Apellidos,
            Correo = u.Correo,
            Telefono = u.Telefono,
            IdRol = u.IdRol,
            NombreRol = u.IdRolNavigation.Nombre,
            Activo = u.Activo,
            FechaRegistro = u.FechaRegistro,
            UltimoAcceso = u.UltimoAcceso
        };
    }

    public async Task CambiarEstadoUsuarioAsync(int id, bool activo)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        usuario.Activo = activo;
        await _context.SaveChangesAsync();
    }

    public async Task ResetPasswordAsync(int id, string nuevaContrasena)
    {
        if (string.IsNullOrWhiteSpace(nuevaContrasena) || nuevaContrasena.Length < 6)
            throw new InvalidOperationException("La contraseña debe tener al menos 6 caracteres");

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        usuario.Contrasena = _passwordHasher.HashPassword(usuario, nuevaContrasena);
        await _context.SaveChangesAsync();
    }
}