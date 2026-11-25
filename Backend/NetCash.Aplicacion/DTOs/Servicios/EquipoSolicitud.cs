using System.ComponentModel.DataAnnotations;

namespace NetCash.Aplicacion.DTOs.Servicios;

public class EquipoSolicitud
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string Descripcion { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ServicioId { get; set; }
}