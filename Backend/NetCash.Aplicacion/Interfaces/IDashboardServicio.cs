using NetCash.Aplicacion.DTOs.Dashboard;
using NetCash.Aplicacion.DTOs.Menus;

namespace NetCash.Aplicacion.Interfaces;

public interface IDashboardServicio
{
    Task<DashboardRespuesta> ObtenerResumenAsync(
        int usuarioId,
        string rol,
        DateTime desde,
        DateTime hasta);

    Task<List<MenuRespuesta>> ObtenerMenuAsync(
        int usuarioId);
}