using NetCash.Aplicacion.DTOs.Servicios;

namespace NetCash.Aplicacion.Interfaces;

public interface ICatalogoServicio
{
    Task<List<ServicioRespuesta>> ObtenerServiciosAsync();

    Task<ServicioRespuesta?> ObtenerServicioAsync(int id);

    Task<ServicioRespuesta> CrearServicioAsync(
        ServicioSolicitud solicitud);

    Task<ServicioRespuesta?> ActualizarServicioAsync(
        int id,
        ServicioSolicitud solicitud);

    Task<bool> EliminarServicioAsync(int id);

    Task<List<EquipoRespuesta>> ObtenerEquiposAsync(
        int? servicioId = null);

    Task<EquipoRespuesta?> ObtenerEquipoAsync(int id);

    Task<EquipoRespuesta> CrearEquipoAsync(
        EquipoSolicitud solicitud);

    Task<EquipoRespuesta?> ActualizarEquipoAsync(
        int id,
        EquipoSolicitud solicitud);

    Task<bool> EliminarEquipoAsync(int id);
}