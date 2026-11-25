namespace NetCash.Aplicacion.DTOs.Turnos;

public class TurnoRespuesta
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public int CajaId { get; set; }

    public string Caja { get; set; } = string.Empty;

    public int CajeroId { get; set; }

    public string Cajero { get; set; } = string.Empty;

    public int GestorId { get; set; }

    public int? ClienteId { get; set; }

    public string? Cliente { get; set; }

    public string TipoAtencion { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public DateTime FechaTurno { get; set; }

    public DateTime? FechaInicioAtencion { get; set; }

    public DateTime? FechaFinAtencion { get; set; }
}