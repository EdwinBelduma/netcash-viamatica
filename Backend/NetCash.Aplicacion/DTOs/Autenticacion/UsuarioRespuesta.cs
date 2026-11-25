namespace NetCash.Aplicacion.DTOs.Autenticacion;

public class UsuarioRespuesta
{
    public int Id { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;
}