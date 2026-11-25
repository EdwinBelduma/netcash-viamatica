using System.ComponentModel.DataAnnotations;

namespace NetCash.Aplicacion.DTOs.Pagos;

public class PagoSolicitud
{
    [Range(1, int.MaxValue)]
    public int ContratoId { get; set; }

    [Range(1, int.MaxValue)]
    public int TurnoId { get; set; }

    [Range(1, int.MaxValue)]
    public int FormaPagoId { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999.99")]
    public decimal Monto { get; set; }

    [StringLength(100)]
    public string? Referencia { get; set; }

    [StringLength(250)]
    public string? Observacion { get; set; }
}