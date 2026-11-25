using NetCash.Dominio.Comunes;
using NetCash.Dominio.Enumeraciones;

namespace NetCash.Dominio.Entidades;

public class Turno : EntidadBase
{
    public string Descripcion { get; set; } = string.Empty;

    public int CajaId { get; set; }

    public Caja Caja { get; set; } = null!;

    public int GestorId { get; set; }

    public Usuario Gestor { get; set; } = null!;

    public int CajeroId { get; set; }

    public Usuario Cajero { get; set; } = null!;

    public int? ClienteId { get; set; }

    public Cliente? Cliente { get; set; }

    public TipoAtencion TipoAtencion { get; set; }

    public EstadoTurno Estado { get; set; }
        = EstadoTurno.Pendiente;

    public DateTime FechaTurno { get; set; }
        = DateTime.UtcNow;

    public DateTime? FechaInicioAtencion { get; set; }

    public DateTime? FechaFinAtencion { get; set; }
}