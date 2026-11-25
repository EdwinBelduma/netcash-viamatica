
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCash.Aplicacion.DTOs.Servicios;
using NetCash.Aplicacion.Interfaces;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/servicios")]
[Authorize(Roles = "Administrador,Gestor,Cajero")]
public class ServiciosController : ControllerBase
{
    private readonly ICatalogoServicio _servicio;
    private readonly ILogger<ServiciosController> _logger;

    public ServiciosController(
        ICatalogoServicio servicio,
        ILogger<ServiciosController> logger)
    {
        _servicio = servicio;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        return Ok(await _servicio.ObtenerServiciosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var servicio = await _servicio
            .ObtenerServicioAsync(id);

        return servicio is null
            ? NotFound()
            : Ok(servicio);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> Crear(
        [FromBody] ServicioSolicitud solicitud)
    {
        try
        {
            var servicio = await _servicio
                .CrearServicioAsync(solicitud);

            _logger.LogInformation(
                "Servicio creado. Id: {Id}. Usuario: {Usuario}",
                servicio.Id,
                User.Identity?.Name);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = servicio.Id },
                servicio);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                "Error al crear servicio: {Mensaje}",
                ex.Message);

            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ServicioSolicitud solicitud)
    {
        try
        {
            var servicio = await _servicio
                .ActualizarServicioAsync(id, solicitud);

            if (servicio is null)
            {
                return NotFound();
            }

            _logger.LogInformation(
                "Servicio actualizado. Id: {Id}. Usuario: {Usuario}",
                id,
                User.Identity?.Name);

            return Ok(servicio);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                "Error al actualizar servicio: {Mensaje}",
                ex.Message);

            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var eliminado = await _servicio
                .EliminarServicioAsync(id);

            if (!eliminado)
            {
                return NotFound();
            }

            _logger.LogInformation(
                "Servicio eliminado. Id: {Id}. Usuario: {Usuario}",
                id,
                User.Identity?.Name);

            return Ok(new
            {
                mensaje = "Servicio eliminado correctamente."
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                "Error al eliminar servicio: {Mensaje}",
                ex.Message);

            return Conflict(new { mensaje = ex.Message });
        }
    }
}
