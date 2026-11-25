namespace NetCash.Aplicacion.DTOs.Autenticacion;

public class LoginRespuesta
{
    public string Token { get; set; } = string.Empty;

    public DateTime FechaExpiracion { get; set; }

    public UsuarioRespuesta Usuario { get; set; } = null!;
}