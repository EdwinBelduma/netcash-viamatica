using NetCash.Aplicacion.DTOs.Contratos;
using NetCash.Aplicacion.DTOs.Pagos;

namespace NetCash.Aplicacion.Interfaces;

public interface IOperacionServicio
{
    Task<List<ContratoRespuesta>> ObtenerContratosAsync(
        int? clienteId = null);

    Task<ContratoRespuesta?> ObtenerContratoAsync(int id);

    Task<ContratoRespuesta> CrearContratoAsync(
        ContratoSolicitud solicitud,
        int cajeroId);

    Task<ContratoRespuesta> CambiarServicioAsync(
        int id,
        CambioServicioSolicitud solicitud,
        int cajeroId);

    Task<ContratoRespuesta> CambiarFormaPagoAsync(
        int id,
        CambioFormaPagoSolicitud solicitud,
        int cajeroId);

    Task<ContratoRespuesta> CancelarContratoAsync(
        int id,
        CancelacionContratoSolicitud solicitud,
        int cajeroId);

    Task<List<PagoRespuesta>> ObtenerPagosAsync(
        int? contratoId = null,
        int? cajeroId = null);

    Task<PagoRespuesta> RegistrarPagoAsync(
        PagoSolicitud solicitud,
        int cajeroId);
}