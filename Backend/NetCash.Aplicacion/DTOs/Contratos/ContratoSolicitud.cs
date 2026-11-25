using System.ComponentModel.DataAnnotations;

namespace NetCash.Aplicacion.DTOs.Contratos;

public class ContratoSolicitud
{
    [Range(1, int.MaxValue)]
    public int ClienteId { get; set; }

    [Range(1, int.MaxValue)]
    public int ServicioId { get; set; }

    [Range(1, int.MaxValue)]
    public int FormaPagoId { get; set; }

    [Range(1, int.MaxValue)]
    public int TurnoId { get; set; }

    [Required]
    public DateTimeOffset FechaFin { get; set; }
}

public class CambioServicioSolicitud
{
    [Range(1, int.MaxValue)]
    public int ServicioId { get; set; }

    [Range(1, int.MaxValue)]
    public int TurnoId { get; set; }
}

public class CambioFormaPagoSolicitud
{
    [Range(1, int.MaxValue)]
    public int FormaPagoId { get; set; }

    [Range(1, int.MaxValue)]
    public int TurnoId { get; set; }
}

public class CancelacionContratoSolicitud
{
    [Required]
    [StringLength(250, MinimumLength = 5)]
    public string Motivo { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TurnoId { get; set; }
}