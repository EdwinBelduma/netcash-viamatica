
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCash.Aplicacion.DTOs.Cajas;
using NetCash.Aplicacion.Interfaces;
using Npgsql;
using System.Security.Claims;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/cajas")]
[Authorize]
public class CajasController : ControllerBase
{
    private readonly ICajaServicio _servicio;
    private readonly ILogger<CajasController> _logger;

    public CajasController(
        ICajaServicio servicio,
        ILogger<CajasController> logger)
    {
        _servicio = servicio;
        _logger = logger;
    }

    private int UsuarioId =>
        int.Parse(User.FindFirstValue(
            ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [Authorize(Roles = "Administrador,Gestor,Cajero")]
    public async Task<IActionResult> ObtenerTodos()
    {
        return Ok(await _servicio.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrador,Gestor,Cajero")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var caja = await _servicio.ObtenerPorIdAsync(id);

        return caja is null
            ? NotFound()
            : Ok(caja);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> Crear(
        [FromBody] CajaSolicitud solicitud)
    {
        try
        {
            var caja = await _servicio.CrearAsync(solicitud);

            _logger.LogInformation(
                "Caja creada. Id: {CajaId}. Usuario: {UsuarioId}",
                caja.Id, UsuarioId);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = caja.Id },
                caja);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] CajaSolicitud solicitud)
    {
        try
        {
            var caja = await _servicio
                .ActualizarAsync(id, solicitud);

            if (caja is null)
            {
                return NotFound();
            }

            _logger.LogInformation(
                "Caja actualizada. Id: {CajaId}. Usuario: {UsuarioId}",
                id, UsuarioId);

            return Ok(caja);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> Eliminar(int id)
    {
        return await EjecutarOperacion(
            () => _servicio.EliminarAsync(id),
            "Caja eliminada correctamente.");
    }

    [HttpPost("{id:int}/cajeros")]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> AsignarCajero(
        int id,
        [FromBody] AsignacionCajaSolicitud solicitud)
    {
        return await EjecutarOperacion(
            () => _servicio.AsignarCajeroAsync(
                id, solicitud.UsuarioId, UsuarioId),
            "Cajero asignado correctamente.");
    }

    [HttpDelete("{id:int}/cajeros/{usuarioId:int}")]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> DesasignarCajero(
        int id,
        int usuarioId)
    {
        return await EjecutarOperacion(
            () => _servicio.DesasignarCajeroAsync(
                id, usuarioId),
            "Cajero desasignado correctamente.");
    }

    [HttpPost("{id:int}/abrir")]
    [Authorize(Roles = "Cajero")]
    public async Task<IActionResult> Abrir(int id)
    {
        return await EjecutarOperacion(
            () => _servicio.AbrirCajaAsync(id, UsuarioId),
            "Caja abierta correctamente.");
    }

    [HttpPost("{id:int}/cerrar")]
    [Authorize(Roles = "Cajero")]
    public async Task<IActionResult> Cerrar(int id)
    {
        return await EjecutarOperacion(
            () => _servicio.CerrarCajaAsync(id, UsuarioId),
            "Caja cerrada correctamente.");
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
                    mensaje = "No se encontro el recurso."
                });
            }

            _logger.LogInformation(
                "{Operacion} UsuarioId: {UsuarioId}",
                mensaje, UsuarioId);

            return Ok(new { mensaje });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
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
