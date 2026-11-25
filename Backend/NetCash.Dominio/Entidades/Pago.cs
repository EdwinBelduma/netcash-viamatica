using NetCash.Dominio.Comunes;

namespace NetCash.Dominio.Entidades;

public class Pago : EntidadBase
{
    public int ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;

    public int ContratoId { get; set; }

    public Contrato Contrato { get; set; } = null!;

    public int CajaId { get; set; }

    public Caja Caja { get; set; } = null!;

    public int CajeroId { get; set; }

    public Usuario Cajero { get; set; } = null!;

    public int FormaPagoId { get; set; }

    public FormaPago FormaPago { get; set; } = null!;

    public decimal Monto { get; set; }

    public DateTime FechaPago { get; set; }
        = DateTime.UtcNow;

    public string? Referencia { get; set; }

    public string? Observacion { get; set; }
}