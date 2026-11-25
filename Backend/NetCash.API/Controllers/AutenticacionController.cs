
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCash.Aplicacion.DTOs.Autenticacion;
using NetCash.Aplicacion.Interfaces;
using System.Security.Claims;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/autenticacion")]
public class AutenticacionController : ControllerBase
{
    private readonly IAutenticacionServicio _autenticacionServicio;
    private readonly ILogger<AutenticacionController> _logger;

    public AutenticacionController(
        IAutenticacionServicio autenticacionServicio,
        ILogger<AutenticacionController> logger)
    {
        _autenticacionServicio = autenticacionServicio;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginSolicitud solicitud)
    {
        var resultado = await _autenticacionServicio
            .IniciarSesionAsync(solicitud);

        if (resultado is null)
        {
            _logger.LogWarning(
                "Intento de inicio de sesion no autorizado.");

            return Unauthorized(new
            {
                mensaje = "Credenciales incorrectas o usuario no habilitado."
            });
        }

        _logger.LogInformation(
            "Inicio de sesion exitoso. UsuarioId: {UsuarioId}",
            resultado.Usuario.Id);

        return Ok(resultado);
    }

    [Authorize]
    [HttpGet("perfil")]
    public IActionResult Perfil()
    {
        return Ok(new
        {
            id = User.FindFirstValue(
                ClaimTypes.NameIdentifier),

            nombreUsuario = User.Identity?.Name,

            rol = User.FindFirstValue(
                ClaimTypes.Role),

            nombres = User.FindFirstValue("nombres"),

            apellidos = User.FindFirstValue("apellidos")
        });
    }
}
