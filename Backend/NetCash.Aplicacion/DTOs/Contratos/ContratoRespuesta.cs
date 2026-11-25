namespace NetCash.Aplicacion.DTOs.Contratos;

public class ContratoRespuesta
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public int ServicioId { get; set; }

    public int FormaPagoId { get; set; }

    public int CajeroId { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public decimal PrecioContratado { get; set; }

    public string Estado { get; set; } = string.Empty;

    public int? ContratoAnteriorId { get; set; }

    public string? MotivoCancelacion { get; set; }

    public bool Activo { get; set; }
}