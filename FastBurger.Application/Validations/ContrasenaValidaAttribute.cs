using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FastBurger.Application.Validations;

public class ContrasenaValidaAttribute : ValidationAttribute, IClientModelValidator
{
    public int MinLength { get; set; } = 6;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var contrasena = value as string;
        if (string.IsNullOrEmpty(contrasena))
            return ValidationResult.Success;

        var errores = new List<string>();

        if (contrasena.Length < MinLength)
            errores.Add("mínimo " + MinLength + " caracteres");

        if (!contrasena.Any(c => char.IsUpper(c)))
            errores.Add("una mayúscula");

        if (!contrasena.Any(c => char.IsLower(c)))
            errores.Add("una minúscula");

        if (!contrasena.Any(char.IsDigit))
            errores.Add("un número");

        if (errores.Count > 0)
            return new ValidationResult("Contraseña inválida, falta: " + string.Join(", ", errores));

        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val"] = "true";
        context.Attributes["data-val-contrasena"] = ErrorMessage ?? "Debe tener mayúscula, minúscula y número";
        context.Attributes["data-val-contrasena-min"] = MinLength.ToString();
    }
}