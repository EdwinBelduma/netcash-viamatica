using System.ComponentModel.DataAnnotations;

namespace NetCash.Aplicacion.DTOs.Servicios;

public class ServicioSolicitud
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Descripcion { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int VelocidadMbps { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999.99")]
    public decimal Precio { get; set; }
}