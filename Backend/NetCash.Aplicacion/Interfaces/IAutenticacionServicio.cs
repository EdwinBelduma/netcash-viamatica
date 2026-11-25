using NetCash.Aplicacion.DTOs.Autenticacion;

namespace NetCash.Aplicacion.Interfaces;

public interface IAutenticacionServicio
{
    Task<LoginRespuesta?> IniciarSesionAsync(
        LoginSolicitud solicitud);
}