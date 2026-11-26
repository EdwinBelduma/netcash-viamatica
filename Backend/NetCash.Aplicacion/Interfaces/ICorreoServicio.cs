namespace NetCash.Aplicacion.Interfaces;

public interface ICorreoServicio
{
    Task EnviarRecuperacionAsync(
        string destinatario,
        string nombres,
        string enlaceRecuperacion);
}