
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCash.Aplicacion.DTOs.Contratos;
using NetCash.Aplicacion.Interfaces;
using Npgsql;
using System.Security.Claims;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/contratos")]
[Authorize(Roles = "Administrador,Gestor,Cajero")]
public class ContratosController : ControllerBase
{
    private readonly IOperacionServicio _servicio;
    private readonly ILogger<ContratosController> _logger;

    public ContratosController(
        IOperacionServicio servicio,
        ILogger<ContratosController> logger)
    {
        _servicio = servicio;
        _logger = logger;
    }

    private int CajeroId =>
        int.Parse(User.FindFirstValue(
            ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(
        [FromQuery] int? clienteId = null)
    {
        return Ok(await _servicio
            .ObtenerContratosAsync(clienteId));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var contrato = await _servicio
            .ObtenerContratoAsync(id);

        return contrato is null
            ? NotFound()
            : Ok(contrato);
    }

    [HttpPost]
    [Authorize(Roles = "Cajero")]
    public async Task<IActionResult> Crear(
        [FromBody] ContratoSolicitud solicitud)
    {
        return await EjecutarOperacion(
            () => _servicio.CrearContratoAsync(
                solicitud, CajeroId),
            "Contrato creado correctamente.",
            true);
    }

    [HttpPatch("{id:int}/cambiar-servicio")]
    [Authorize(Roles = "Cajero")]
    public async Task<IActionResult> CambiarServicio(
        int id,
        [FromBody] CambioServicioSolicitud solicitud)
    {
        return await EjecutarOperacion(
            () => _servicio.CambiarServicioAsync(
                id, solicitud, CajeroId),
            "Servicio cambiado correctamente.");
    }

    [HttpPatch("{id:int}/cambiar-forma-pago")]
    [Authorize(Roles = "Cajero")]
    public async Task<IActionResult> CambiarFormaPago(
        int id,
        [FromBody] CambioFormaPagoSolicitud solicitud)
    {
        return await EjecutarOperacion(
            () => _servicio.CambiarFormaPagoAsync(
                id, solicitud, CajeroId),
            "Forma de pago modificada correctamente.");
    }

    [HttpPatch("{id:int}/cancelar")]
    [Authorize(Roles = "Cajero")]
    public async Task<IActionResult> Cancelar(
        int id,
        [FromBody] CancelacionContratoSolicitud solicitud)
    {
        return await EjecutarOperacion(
            () => _servicio.CancelarContratoAsync(
                id, solicitud, CajeroId),
            "Contrato cancelado correctamente.");
    }

    private async Task<IActionResult> EjecutarOperacion(
        Func<Task<ContratoRespuesta>> operacion,
        string mensaje,
        bool creado = false)
    {
        try
        {
            var contrato = await operacion();

            _logger.LogInformation(
                "{Operacion}. ContratoId: {ContratoId}. CajeroId: {CajeroId}",
                mensaje,
                contrato.Id,
                CajeroId);

            if (creado)
            {
                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id = contrato.Id },
                    contrato);
            }

            return Ok(contrato);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                "Operacion contractual rechazada: {Mensaje}",
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
                "Conflicto de concurrencia en contratos.");

            return Conflict(new
            {
                mensaje = "Operacion concurrente. Intente nuevamente."
            });
        }
    }
}
