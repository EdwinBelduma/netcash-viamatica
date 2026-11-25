using NetCash.Dominio.Comunes;
using NetCash.Dominio.Enumeraciones;

namespace NetCash.Dominio.Entidades;

public class Contrato : EntidadBase
{
    public int ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;

    public int ServicioId { get; set; }

    public Servicio Servicio { get; set; } = null!;

    public int FormaPagoId { get; set; }

    public FormaPago FormaPago { get; set; } = null!;

    public int CajeroId { get; set; }

    public Usuario Cajero { get; set; } = null!;

    public DateTime FechaInicio { get; set; }
        = DateTime.UtcNow;

    public DateTime FechaFin { get; set; }

    public EstadoContrato Estado { get; set; }
        = EstadoContrato.Vigente;

    public decimal PrecioContratado { get; set; }

    public int? ContratoAnteriorId { get; set; }

    public Contrato? ContratoAnterior { get; set; }

    public string? MotivoCancelacion { get; set; }
}