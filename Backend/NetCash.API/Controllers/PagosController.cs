
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCash.Aplicacion.DTOs.Pagos;
using NetCash.Aplicacion.Interfaces;
using Npgsql;
using System.Security.Claims;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/pagos")]
[Authorize(Roles = "Administrador,Gestor,Cajero")]
public class PagosController : ControllerBase
{
    private readonly IOperacionServicio _servicio;
    private readonly ILogger<PagosController> _logger;

    public PagosController(
        IOperacionServicio servicio,
        ILogger<PagosController> logger)
    {
        _servicio = servicio;
        _logger = logger;
    }

    private int UsuarioId =>
        int.Parse(User.FindFirstValue(
            ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(
        [FromQuery] int? contratoId = null)
    {
        int? cajeroId = User.IsInRole("Cajero")
            ? UsuarioId
            : null;

        var pagos = await _servicio
            .ObtenerPagosAsync(
                contratoId,
                cajeroId);

        return Ok(pagos);
    }

    [HttpPost]
    [Authorize(Roles = "Cajero")]
    public async Task<IActionResult> Registrar(
        [FromBody] PagoSolicitud solicitud)
    {
        try
        {
            var pago = await _servicio
                .RegistrarPagoAsync(
                    solicitud,
                    UsuarioId);

            _logger.LogInformation(
                "Pago registrado. PagoId: {PagoId}. CajeroId: {CajeroId}. CajaId: {CajaId}",
                pago.Id,
                UsuarioId,
                pago.CajaId);

            return StatusCode(
                StatusCodes.Status201Created,
                pago);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                "Pago rechazado: {Mensaje}",
                ex.Message);

            return Conflict(new
            {
                mensaje = ex.Message
            });
        }
        catch (Exception ex) when (
            ex.GetBaseException() is PostgresException pg &&
            pg.SqlState == "40001")
        {
            _logger.LogWarning(
                "Conflicto de concurrencia en pagos.");

            return Conflict(new
            {
                mensaje = "Operacion concurrente. Intente nuevamente."
            });
        }
    }
}
