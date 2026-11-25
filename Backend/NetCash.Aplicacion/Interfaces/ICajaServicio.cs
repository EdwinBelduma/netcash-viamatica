using NetCash.Aplicacion.DTOs.Cajas;

namespace NetCash.Aplicacion.Interfaces;

public interface ICajaServicio
{
    Task<List<CajaRespuesta>> ObtenerTodosAsync();

    Task<CajaRespuesta?> ObtenerPorIdAsync(int id);

    Task<CajaRespuesta> CrearAsync(
        CajaSolicitud solicitud);

    Task<CajaRespuesta?> ActualizarAsync(
        int id,
        CajaSolicitud solicitud);

    Task<bool> EliminarAsync(int id);

    Task<bool> AsignarCajeroAsync(
        int cajaId,
        int usuarioId,
        int gestorId);

    Task<bool> DesasignarCajeroAsync(
        int cajaId,
        int usuarioId);

    Task<bool> AbrirCajaAsync(
        int cajaId,
        int usuarioId);

    Task<bool> CerrarCajaAsync(
        int cajaId,
        int usuarioId);
}