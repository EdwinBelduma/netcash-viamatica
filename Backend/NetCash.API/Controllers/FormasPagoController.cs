using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCash.Infraestructura.Persistencia;

namespace NetCash.API.Controllers;

[ApiController]
[Route("api/formas-pago")]
[Authorize(Roles = "Administrador,Gestor,Cajero")]
public class FormasPagoController : ControllerBase
{
    private readonly NetCashDbContext _context;

    public FormasPagoController(
        NetCashDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var formas = await _context.FormasPago
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Id)
            .Select(x => new
            {
                x.Id,
                x.Nombre,
                x.Descripcion
            })
            .ToListAsync();

        return Ok(formas);
    }
}