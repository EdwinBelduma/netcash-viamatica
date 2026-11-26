using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NetCash.Aplicacion.DTOs.Autenticacion;
using NetCash.Aplicacion.Interfaces;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/recuperacion")]
[AllowAnonymous]
[EnableRateLimiting("Recuperacion")]
public class RecuperacionController : ControllerBase
{
    private readonly IRecuperacionServicio _recuperacionServicio;

    public RecuperacionController(
        IRecuperacionServicio recuperacionServicio)
    {
        _recuperacionServicio = recuperacionServicio;
    }

    // ==========================================
    // SOLICITAR RECUPERACION
    // ==========================================

    [HttpPost("solicitar")]
    public async Task<IActionResult> SolicitarRecuperacion(
        [FromBody] RecuperarContrasenaSolicitud solicitud)
    {
        await _recuperacionServicio
            .SolicitarRecuperacionAsync(solicitud);

        return Ok(new
        {
            mensaje = "Si el correo está registrado, " +
                      "recibirás un enlace para restablecer " +
                      "tu contraseña."
        });
    }

    // ==========================================
    // RESTABLECER CONTRASENA
    // ==========================================

    [HttpPost("restablecer")]
    public async Task<IActionResult> RestablecerContrasena(
        [FromBody] RestablecerContrasenaSolicitud solicitud)
    {
        var resultado = await _recuperacionServicio
            .RestablecerContrasenaAsync(solicitud);

        if (!resultado)
        {
            return BadRequest(new
            {
                mensaje = "El enlace de recuperación no es válido, " +
                          "ha expirado o ya fue utilizado."
            });
        }

        return Ok(new
        {
            mensaje = "Contraseña actualizada correctamente."
        });
    }
}