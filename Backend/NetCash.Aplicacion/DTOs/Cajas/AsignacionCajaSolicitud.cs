using System.ComponentModel.DataAnnotations;

namespace NetCash.Aplicacion.DTOs.Cajas;

public class AsignacionCajaSolicitud
{
    [Range(1, int.MaxValue)]
    public int UsuarioId { get; set; }
}