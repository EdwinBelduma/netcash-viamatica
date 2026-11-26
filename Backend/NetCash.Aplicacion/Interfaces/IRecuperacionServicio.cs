using NetCash.Aplicacion.DTOs.Autenticacion;

namespace NetCash.Aplicacion.Interfaces;

public interface IRecuperacionServicio
{
    Task SolicitarRecuperacionAsync(
        RecuperarContrasenaSolicitud solicitud);

    Task<bool> RestablecerContrasenaAsync(
        RestablecerContrasenaSolicitud solicitud);
}