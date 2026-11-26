using System.ComponentModel.DataAnnotations;

namespace NetCash.Aplicacion.DTOs.Autenticacion;

public class RecuperarContrasenaSolicitud
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no es válido.")]
    [StringLength(120)]
    public string Correo { get; set; } = string.Empty;
}