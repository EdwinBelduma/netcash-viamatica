
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCash.Aplicacion.Interfaces;
using System.Security.Claims;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Administrador,Gestor,Cajero")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardServicio _servicio;

    public DashboardController(
        IDashboardServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> ObtenerResumen(
        [FromQuery] DateOnly? desde,
        [FromQuery] DateOnly? hasta)
    {
        var hoyEcuador = DateOnly.FromDateTime(
            DateTime.UtcNow.AddHours(-5));

        var fechaDesde = desde ?? hoyEcuador;
        var fechaHasta = hasta ?? hoyEcuador;

        if (fechaDesde > fechaHasta)
        {
            return BadRequest(new
            {
                mensaje = "El rango de fechas no es valido."
            });
        }

        var inicio = DateTime.SpecifyKind(
            fechaDesde.ToDateTime(TimeOnly.MinValue)
                .AddHours(5),
            DateTimeKind.Utc);

        var fin = DateTime.SpecifyKind(
            fechaHasta.AddDays(1)
                .ToDateTime(TimeOnly.MinValue)
                .AddHours(5),
            DateTimeKind.Utc);

        var usuarioId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var rol = User.FindFirstValue(
            ClaimTypes.Role) ?? string.Empty;

        var resultado = await _servicio
            .ObtenerResumenAsync(
                usuarioId,
                rol,
                inicio,
                fin);

        return Ok(resultado);
    }
}
