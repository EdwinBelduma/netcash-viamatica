using System.ComponentModel.DataAnnotations;

namespace NetCash.Aplicacion.DTOs.Autenticacion;

public class RestablecerContrasenaSolicitud
{
    [Required(ErrorMessage = "El token es obligatorio.")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(30, MinimumLength = 8)]
    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*[0-9]).{8,30}$",
        ErrorMessage = "Debe contener una mayúscula y un número.")]
    public string NuevaContrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma la contraseña.")]
    [Compare(
        nameof(NuevaContrasena),
        ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmarContrasena { get; set; } = string.Empty;
}