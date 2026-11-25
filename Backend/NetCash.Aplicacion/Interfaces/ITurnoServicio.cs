using NetCash.Aplicacion.DTOs.Turnos;

namespace NetCash.Aplicacion.Interfaces;

public interface ITurnoServicio
{
    Task<List<TurnoRespuesta>> ObtenerTodosAsync(
        int? cajeroId = null);

    Task<TurnoRespuesta?> ObtenerPorIdAsync(
        int id,
        int? cajeroId = null);

    Task<TurnoRespuesta> CrearAsync(
        TurnoSolicitud solicitud,
        int gestorId);

    Task<bool> IniciarAtencionAsync(
        int id,
        int cajeroId);

    Task<bool> FinalizarAtencionAsync(
        int id,
        int cajeroId);

    Task<bool> CancelarAsync(int id);

    Task<bool> EliminarAsync(int id);
}