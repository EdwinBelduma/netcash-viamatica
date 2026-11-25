using System.ComponentModel.DataAnnotations;

namespace NetCash.Aplicacion.DTOs.Cajas;

public class CajaSolicitud
{
    [Required(ErrorMessage = "La descripcion es obligatoria.")]
    [StringLength(50, MinimumLength = 3)]
    public string Descripcion { get; set; } = string.Empty;
}