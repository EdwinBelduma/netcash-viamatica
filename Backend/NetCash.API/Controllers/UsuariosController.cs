
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCash.Aplicacion.DTOs.Usuarios;
using NetCash.Aplicacion.Interfaces;
using System.Security.Claims;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "Administrador")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioServicio _servicio;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(
        IUsuarioServicio servicio,
        ILogger<UsuariosController> logger)
    {
        _servicio = servicio;
        _logger = logger;
    }

    private int AdministradorId =>
        int.Parse(User.FindFirstValue(
            ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        return Ok(await _servicio.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var usuario = await _servicio.ObtenerPorIdAsync(id);

        if (usuario is null)
        {
            return NotFound(new
            {
                mensaje = "Usuario no encontrado."
            });
        }

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] UsuarioSolicitud solicitud)
    {
        try
        {
            var usuario = await _servicio.CrearAsync(
                solicitud,
                AdministradorId);

            _logger.LogInformation(
                "Usuario creado. Id: {UsuarioId}. Administrador: {AdministradorId}",
                usuario.Id,
                AdministradorId);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = usuario.Id },
                usuario);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] UsuarioActualizarSolicitud solicitud)
    {
        try
        {
            var usuario = await _servicio.ActualizarAsync(
                id,
                solicitud,
                AdministradorId);

            if (usuario is null)
            {
                return NotFound();
            }

            _logger.LogInformation(
                "Usuario actualizado. Id: {UsuarioId}. Administrador: {AdministradorId}",
                id,
                AdministradorId);

            return Ok(usuario);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpPatch("{id:int}/aprobar")]
    public async Task<IActionResult> Aprobar(int id)
    {
        try
        {
            var aprobado = await _servicio.AprobarAsync(
                id,
                AdministradorId);

            if (!aprobado)
            {
                return NotFound();
            }

            _logger.LogInformation(
                "Usuario aprobado. Id: {UsuarioId}. Administrador: {AdministradorId}",
                id,
                AdministradorId);

            return Ok(new
            {
                mensaje = "Usuario aprobado correctamente."
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var eliminado = await _servicio.EliminarAsync(
                id,
                AdministradorId);

            if (!eliminado)
            {
                return NotFound();
            }

            _logger.LogInformation(
                "Usuario eliminado logicamente. Id: {UsuarioId}. Administrador: {AdministradorId}",
                id,
                AdministradorId);

            return Ok(new
            {
                mensaje = "Usuario eliminado correctamente."
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }
}
