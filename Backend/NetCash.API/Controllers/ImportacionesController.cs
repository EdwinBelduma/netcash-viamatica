using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCash.Aplicacion.Interfaces;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/importaciones")]
[Authorize]
public class ImportacionesController : ControllerBase
{
    private readonly IImportacionServicio _importacionServicio;

    public ImportacionesController(
        IImportacionServicio importacionServicio)
    {
        _importacionServicio = importacionServicio;
    }

    [HttpPost("usuarios")]
    [Authorize(Roles = "Administrador")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5242880)]
    public async Task<IActionResult> ImportarUsuarios(
        IFormFile archivo)
    {
        var error = ValidarArchivo(archivo);

        if (error != null)
        {
            return BadRequest(new { mensaje = error });
        }

        if (!ObtenerUsuarioId(out int usuarioId))
        {
            return Unauthorized();
        }

        using var contenido = archivo.OpenReadStream();

        var resultado =
            await _importacionServicio.ImportarUsuariosAsync(
                contenido,
                usuarioId);

        return Ok(resultado);
    }

    [HttpPost("clientes")]
    [Authorize(Roles = "Administrador,Cajero")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5242880)]
    public async Task<IActionResult> ImportarClientes(
        IFormFile archivo)
    {
        var error = ValidarArchivo(archivo);

        if (error != null)
        {
            return BadRequest(new { mensaje = error });
        }

        if (!ObtenerUsuarioId(out int usuarioId))
        {
            return Unauthorized();
        }

        using var contenido = archivo.OpenReadStream();

        var resultado =
            await _importacionServicio.ImportarClientesAsync(
                contenido,
                usuarioId);

        return Ok(resultado);
    }

    private bool ObtenerUsuarioId(out int usuarioId)
    {
        var identificador =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return int.TryParse(identificador, out usuarioId);
    }

    private static string? ValidarArchivo(IFormFile? archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            return "Debe seleccionar un archivo CSV.";
        }

        if (!string.Equals(
            Path.GetExtension(archivo.FileName),
            ".csv",
            StringComparison.OrdinalIgnoreCase))
        {
            return "Solo se permiten archivos CSV.";
        }

        if (archivo.Length > 5 * 1024 * 1024)
        {
            return "El archivo no puede superar los 5 MB.";
        }

        return null;
    }
}