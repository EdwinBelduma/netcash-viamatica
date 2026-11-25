using NetCash.Aplicacion.DTOs.Usuarios;

namespace NetCash.Aplicacion.Interfaces;

public interface IUsuarioServicio
{
    Task<List<UsuarioDetalleRespuesta>> ObtenerTodosAsync();

    Task<UsuarioDetalleRespuesta?> ObtenerPorIdAsync(int id);

    Task<UsuarioDetalleRespuesta> CrearAsync(
        UsuarioSolicitud solicitud,
        int administradorId);

    Task<UsuarioDetalleRespuesta?> ActualizarAsync(
        int id,
        UsuarioActualizarSolicitud solicitud,
        int administradorId);

    Task<bool> AprobarAsync(
        int id,
        int administradorId);

    Task<bool> EliminarAsync(
        int id,
        int administradorId);
}