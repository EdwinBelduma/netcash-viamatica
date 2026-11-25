using NetCash.Dominio.Comunes;
using NetCash.Dominio.Enumeraciones;

namespace NetCash.Dominio.Entidades;

public class Atencion : EntidadBase
{
    public int TurnoId { get; set; }

    public Turno Turno { get; set; } = null!;

    public int ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;

    public int CajeroId { get; set; }

    public Usuario Cajero { get; set; } = null!;

    public TipoAtencion Tipo { get; set; }

    public EstadoTurno Estado { get; set; }

    public string? Observacion { get; set; }

    public DateTime FechaAtencion { get; set; }
        = DateTime.UtcNow;
}