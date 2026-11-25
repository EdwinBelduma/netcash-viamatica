namespace NetCash.Aplicacion.DTOs.Dashboard;

public class DashboardRespuesta
{
    public int TotalClientes { get; set; }

    public int TotalUsuarios { get; set; }

    public int TotalCajas { get; set; }

    public int CajasOcupadas { get; set; }

    public int CajasDisponibles { get; set; }

    public int CajerosActivos { get; set; }

    public int GestoresActivos { get; set; }

    public int UsuariosPendientes { get; set; }

    public int SesionesActivas { get; set; }

    public int TurnosPendientes { get; set; }

    public int TurnosEnAtencion { get; set; }

    public int TurnosAtendidos { get; set; }

    public int ContratosVigentes { get; set; }

    public int TotalPagos { get; set; }

    public decimal TotalRecaudado { get; set; }

    public List<IndicadorRespuesta> IndicadoresCajas { get; set; } = new();

    public List<IndicadorRespuesta> IndicadoresCajeros { get; set; } = new();

    public List<IndicadorRespuesta> IndicadoresGestores { get; set; } = new();

    public List<SesionRespuesta> Sesiones { get; set; } = new();
}

public class IndicadorRespuesta
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public int TurnosAtendidos { get; set; }
}

public class SesionRespuesta
{
    public int Id { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public bool SesionActiva { get; set; }

    public DateTime? UltimoInicioSesion { get; set; }
}