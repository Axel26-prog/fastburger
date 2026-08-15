using FastBurger.Application.DTOs;
using FastBurger.Infrastructure.Models;

namespace FastBurger.Application.Interfaces;

public interface IAutenticacionService
{
    Task<ResultadoLoginDTO> LoginAsync(string correo, string contrasena);

    Task<Usuario> RegistrarClienteAsync(RegistroClienteDTO dto);

    Task<Usuario> CrearStaffAsync(CrearStaffDTO dto);

    Task<List<UsuarioListaDTO>> ListarUsuariosAsync();

    Task<List<UsuarioListaDTO>> ListarClientesAsync();

    Task<UsuarioListaDTO?> ObtenerUsuarioAsync(int id);

    Task CambiarEstadoUsuarioAsync(int id, bool activo);

    Task ResetPasswordAsync(int id, string nuevaContrasena);
}