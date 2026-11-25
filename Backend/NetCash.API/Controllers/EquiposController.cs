
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCash.Aplicacion.DTOs.Servicios;
using NetCash.Aplicacion.Interfaces;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/equipos")]
[Authorize(Roles = "Administrador,Gestor,Cajero")]
public class EquiposController : ControllerBase
{
    private readonly ICatalogoServicio _servicio;
    private readonly ILogger<EquiposController> _logger;

    public EquiposController(
        ICatalogoServicio servicio,
        ILogger<EquiposController> logger)
    {
        _servicio = servicio;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(
        [FromQuery] int? servicioId = null)
    {
        return Ok(await _servicio
            .ObtenerEquiposAsync(servicioId));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var equipo = await _servicio
            .ObtenerEquipoAsync(id);

        return equipo is null
            ? NotFound()
            : Ok(equipo);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> Crear(
        [FromBody] EquipoSolicitud solicitud)
    {
        try
        {
            var equipo = await _servicio
                .CrearEquipoAsync(solicitud);

            _logger.LogInformation(
                "Equipo creado. Id: {Id}. Usuario: {Usuario}",
                equipo.Id,
                User.Identity?.Name);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = equipo.Id },
                equipo);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                "Error al crear equipo: {Mensaje}",
                ex.Message);

            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] EquipoSolicitud solicitud)
    {
        try
        {
            var equipo = await _servicio
                .ActualizarEquipoAsync(id, solicitud);

            if (equipo is null)
            {
                return NotFound();
            }

            _logger.LogInformation(
                "Equipo actualizado. Id: {Id}. Usuario: {Usuario}",
                id,
                User.Identity?.Name);

            return Ok(equipo);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                "Error al actualizar equipo: {Mensaje}",
                ex.Message);

            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _servicio
            .EliminarEquipoAsync(id);

        if (!eliminado)
        {
            return NotFound();
        }

        _logger.LogInformation(
            "Equipo eliminado. Id: {Id}. Usuario: {Usuario}",
            id,
            User.Identity?.Name);

        return Ok(new
        {
            mensaje = "Equipo eliminado correctamente."
        });
    }
}
