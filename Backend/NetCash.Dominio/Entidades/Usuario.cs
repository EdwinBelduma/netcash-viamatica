using NetCash.Dominio.Comunes;
using NetCash.Dominio.Enumeraciones;

namespace NetCash.Dominio.Entidades;

public class Usuario : EntidadBase
{
    public string NombreUsuario { get; set; }
        = string.Empty;

    public string Nombres { get; set; }
        = string.Empty;

    public string Apellidos { get; set; }
        = string.Empty;

    public string IdentificacionCifrada { get; set; }
        = string.Empty;

    public string CorreoCifrado { get; set; }
        = string.Empty;

    public string ContrasenaHash { get; set; }
        = string.Empty;

    public int RolId { get; set; }

    public Rol Rol { get; set; } = null!;

    public EstadoUsuario Estado { get; set; }
        = EstadoUsuario.PendienteAprobacion;

    public int? UsuarioCreacionId { get; set; }

    public int? UsuarioAprobacionId { get; set; }

    public DateTime? FechaAprobacion { get; set; }

    public DateTime? UltimoInicioSesion { get; set; }

    public bool SesionActiva { get; set; }
}