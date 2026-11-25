using NetCash.Aplicacion.DTOs.Clientes;

namespace NetCash.Aplicacion.Interfaces;

public interface IClienteServicio
{
    Task<List<ClienteRespuesta>> ObtenerTodosAsync();

    Task<ClienteRespuesta?> ObtenerPorIdAsync(int id);

    Task<ClienteRespuesta> CrearAsync(
        ClienteSolicitud solicitud);

    Task<ClienteRespuesta?> ActualizarAsync(
        int id,
        ClienteSolicitud solicitud);

    Task<bool> EliminarAsync(int id);
}