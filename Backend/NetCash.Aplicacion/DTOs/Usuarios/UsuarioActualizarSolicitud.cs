using System.ComponentModel.DataAnnotations;

namespace NetCash.Aplicacion.DTOs.Usuarios;

public class UsuarioActualizarSolicitud
{
    [Required]
    [StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{10}$")]
    public string Identificacion { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Correo { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int RolId { get; set; }
}