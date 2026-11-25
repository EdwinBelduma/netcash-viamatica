
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCash.Aplicacion.DTOs.Clientes;
using NetCash.Aplicacion.Interfaces;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize(Roles = "Administrador,Cajero")]
public class ClientesController : ControllerBase
{
    private readonly IClienteServicio _servicio;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(
        IClienteServicio servicio,
        ILogger<ClientesController> logger)
    {
        _servicio = servicio;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        return Ok(await _servicio.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var cliente = await _servicio.ObtenerPorIdAsync(id);

        if (cliente is null)
        {
            return NotFound(new
            {
                mensaje = "Cliente no encontrado."
            });
        }

        return Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] ClienteSolicitud solicitud)
    {
        try
        {
            var cliente = await _servicio.CrearAsync(solicitud);

            _logger.LogInformation(
                "Cliente creado. Id: {ClienteId}. Usuario: {Usuario}",
                cliente.Id,
                User.Identity?.Name);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = cliente.Id },
                cliente);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ClienteSolicitud solicitud)
    {
        try
        {
            var cliente = await _servicio
                .ActualizarAsync(id, solicitud);

            if (cliente is null)
            {
                return NotFound(new
                {
                    mensaje = "Cliente no encontrado."
                });
            }

            _logger.LogInformation(
                "Cliente actualizado. Id: {ClienteId}. Usuario: {Usuario}",
                id,
                User.Identity?.Name);

            return Ok(cliente);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _servicio.EliminarAsync(id);

        if (!eliminado)
        {
            return NotFound(new
            {
                mensaje = "Cliente no encontrado."
            });
        }

        _logger.LogInformation(
            "Cliente eliminado logicamente. Id: {ClienteId}. Usuario: {Usuario}",
            id,
            User.Identity?.Name);

        return Ok(new
        {
            mensaje = "Cliente eliminado correctamente."
        });
    }
}
