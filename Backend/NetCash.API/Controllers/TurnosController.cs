
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCash.Aplicacion.DTOs.Turnos;
using NetCash.Aplicacion.Interfaces;
using Npgsql;
using System.Security.Claims;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/turnos")]
[Authorize(Roles = "Administrador,Gestor,Cajero")]
public class TurnosController : ControllerBase
{
    private readonly ITurnoServicio _servicio;
    private readonly ILogger<TurnosController> _logger;

    public TurnosController(
        ITurnoServicio servicio,
        ILogger<TurnosController> logger)
    {
        _servicio = servicio;
        _logger = logger;
    }

    private int UsuarioId =>
        int.Parse(User.FindFirstValue(
            ClaimTypes.NameIdentifier)!);

    private int? FiltroCajero =>
        User.IsInRole("Cajero") ? UsuarioId : null;

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        return Ok(await _servicio
            .ObtenerTodosAsync(FiltroCajero));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var turno = await _servicio
            .ObtenerPorIdAsync(id, FiltroCajero);

        return turno is null
            ? NotFound()
            : Ok(turno);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> Crear(
        [FromBody] TurnoSolicitud solicitud)
    {
        try
        {
            var turno = await _servicio.CrearAsync(
                solicitud,
                UsuarioId);

            _logger.LogInformation(
                "Turno creado. Id: {TurnoId}. Usuario: {UsuarioId}",
                turno.Id,
                UsuarioId);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = turno.Id },
                turno);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
        catch (Exception ex) when (
            ex.GetBaseException() is PostgresException pg &&
            (pg.SqlState == "40001" ||
             pg.SqlState == "23505"))
        {
            return Conflict(new
            {
                mensaje = "Operacion concurrente. Intente nuevamente."
            });
        }
    }

    [HttpPatch("{id:int}/iniciar")]
    [Authorize(Roles = "Cajero")]
    public async Task<IActionResult> Iniciar(int id)
    {
        return await EjecutarOperacion(
            () => _servicio.IniciarAtencionAsync(
                id, UsuarioId),
            "Atencion iniciada correctamente.");
    }

    [HttpPatch("{id:int}/finalizar")]
    [Authorize(Roles = "Cajero")]
    public async Task<IActionResult> Finalizar(int id)
    {
        return await EjecutarOperacion(
            () => _servicio.FinalizarAtencionAsync(
                id, UsuarioId),
            "Atencion finalizada correctamente.");
    }

    [HttpPatch("{id:int}/cancelar")]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> Cancelar(int id)
    {
        return await EjecutarOperacion(
            () => _servicio.CancelarAsync(id),
            "Turno cancelado correctamente.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> Eliminar(int id)
    {
        return await EjecutarOperacion(
            () => _servicio.EliminarAsync(id),
            "Turno eliminado correctamente.");
    }

    private async Task<IActionResult> EjecutarOperacion(
        Func<Task<bool>> operacion,
        string mensaje)
    {
        try
        {
            if (!await operacion())
            {
                return NotFound(new
                {
                    mensaje = "Turno no encontrado."
                });
            }

            _logger.LogInformation(
                "{Operacion}. Usuario: {UsuarioId}",
                mensaje,
                UsuarioId);

            return Ok(new { mensaje });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                mensaje = ex.Message
            });
        }
        catch (Exception ex) when (
            ex.GetBaseException() is PostgresException pg &&
            pg.SqlState == "40001")
        {
            return Conflict(new
            {
                mensaje = "Operacion concurrente. Intente nuevamente."
            });
        }
    }
}
