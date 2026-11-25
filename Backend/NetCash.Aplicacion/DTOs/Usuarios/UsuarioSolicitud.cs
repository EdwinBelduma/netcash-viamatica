using System.ComponentModel.DataAnnotations;

namespace NetCash.Aplicacion.DTOs.Usuarios;

public class UsuarioSolicitud
{
    [Required]
    [StringLength(20, MinimumLength = 8)]
    [RegularExpression(@"^[a-zA-Z0-9]+$")]
    public string NombreUsuario { get; set; } = string.Empty;

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

    [Required]
    [StringLength(30, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$")]
    public string Contrasena { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int RolId { get; set; }
}