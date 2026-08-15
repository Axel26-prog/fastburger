using System.ComponentModel.DataAnnotations;
using FastBurger.Application.Validations;

namespace FastBurger.Application.DTOs;

public class LoginDTO
{
    [Required(ErrorMessage = "Ingrese su correo")]
    [EmailAddress(ErrorMessage = "Ingrese un correo v\u00e1lido")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese su contrase\u00f1a")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = null!;

    public bool Recordarme { get; set; }

    public string? ReturnUrl { get; set; }
}

public class RegistroClienteDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(50, ErrorMessage = "M\u00e1ximo 50 caracteres")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(50, ErrorMessage = "M\u00e1ximo 50 caracteres")]
    public string Apellidos { get; set; } = null!;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un correo v\u00e1lido")]
    public string Correo { get; set; } = null!;

    [StringLength(20, ErrorMessage = "M\u00e1ximo 20 caracteres")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "La contrase\u00f1a es obligatoria")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "M\u00ednimo 6 caracteres")]
    [ContrasenaValida(ErrorMessage = "La contrase\u00f1a debe tener may\u00fascula, min\u00fascula y n\u00famero")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = null!;

    [Required(ErrorMessage = "Debe confirmar la contrase\u00f1a")]
    [Compare("Contrasena", ErrorMessage = "Las contrase\u00f1as no coinciden")]
    [DataType(DataType.Password)]
    public string ConfirmarContrasena { get; set; } = null!;
}

public class CrearStaffDTO
{
    [Required(ErrorMessage = "Seleccione un rol")]
    public string Rol { get; set; } = null!;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(50, ErrorMessage = "M\u00e1ximo 50 caracteres")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(50, ErrorMessage = "M\u00e1ximo 50 caracteres")]
    public string Apellidos { get; set; } = null!;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un correo v\u00e1lido")]
    public string Correo { get; set; } = null!;

    [StringLength(20, ErrorMessage = "M\u00e1ximo 20 caracteres")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "La contrase\u00f1a es obligatoria")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "M\u00ednimo 6 caracteres")]
    [ContrasenaValida(ErrorMessage = "La contrase\u00f1a debe tener may\u00fascula, min\u00fascula y n\u00famero")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = null!;

    [Required(ErrorMessage = "Debe confirmar la contrase\u00f1a")]
    [Compare("Contrasena", ErrorMessage = "Las contrase\u00f1as no coinciden")]
    [DataType(DataType.Password)]
    public string ConfirmarContrasena { get; set; } = null!;
}

public class ResultadoLoginDTO
{
    public bool Exito { get; set; }
    public string? Error { get; set; }

    public int IdUsuario { get; set; }
    public string? Nombre { get; set; }
    public string? Apellidos { get; set; }
    public string? Correo { get; set; }
    public int IdRol { get; set; }
    public string? NombreRol { get; set; }
}

public class UsuarioListaDTO
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string? Telefono { get; set; }
    public int IdRol { get; set; }
    public string? NombreRol { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? UltimoAcceso { get; set; }

    public string NombreCompleto
    {
        get { return Nombre + " " + Apellidos; }
    }
}