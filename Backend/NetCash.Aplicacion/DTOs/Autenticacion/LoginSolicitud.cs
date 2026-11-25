using System.ComponentModel.DataAnnotations;

namespace NetCash.Aplicacion.DTOs.Autenticacion;

public class LoginSolicitud
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(20, MinimumLength = 8)]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(30, MinimumLength = 8)]
    public string Contrasena { get; set; } = string.Empty;
}