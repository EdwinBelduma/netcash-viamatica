using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCash.Aplicacion.Interfaces;
using System.Security.Claims;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/menus")]
[Authorize]
public class MenusController : ControllerBase
{
    private readonly IDashboardServicio _servicio;

    public MenusController(
        IDashboardServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet("mi-menu")]
    public async Task<IActionResult> ObtenerMenu()
    {
        var usuarioId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var menu = await _servicio
            .ObtenerMenuAsync(usuarioId);

        return Ok(menu);
    }
}