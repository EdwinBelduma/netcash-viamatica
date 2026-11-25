namespace NetCash.Aplicacion.DTOs.Usuarios;

public class UsuarioDetalleRespuesta
{
    public int Id { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string Identificacion { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public int RolId { get; set; }

    public string Rol { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaAprobacion { get; set; }
}