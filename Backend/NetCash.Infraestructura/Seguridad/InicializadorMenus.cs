
using Microsoft.EntityFrameworkCore;
using NetCash.Dominio.Entidades;
using NetCash.Infraestructura.Persistencia;

namespace NetCash.Infraestructura.Seguridad;

public class InicializadorMenus
{
    private readonly NetCashDbContext _context;

    public InicializadorMenus(NetCashDbContext context)
    {
        _context = context;
    }

    public async Task InicializarAsync()
    {
        var configuracion = new[]
        {
            new { Nombre = "Dashboard", Ruta = "/dashboard",
                Icono = "layout-dashboard", Orden = 1,
                Roles = new[] { 1, 2, 3 } },

            new { Nombre = "Usuarios", Ruta = "/usuarios",
                Icono = "users", Orden = 2,
                Roles = new[] { 1 } },

            new { Nombre = "Clientes", Ruta = "/clientes",
                Icono = "contact", Orden = 3,
                Roles = new[] { 1, 3 } },

            new { Nombre = "Cajas", Ruta = "/cajas",
                Icono = "wallet", Orden = 4,
                Roles = new[] { 1, 2, 3 } },

            new { Nombre = "Turnos", Ruta = "/turnos",
                Icono = "clock", Orden = 5,
                Roles = new[] { 1, 2, 3 } },

            new { Nombre = "Servicios", Ruta = "/servicios",
                Icono = "wifi", Orden = 6,
                Roles = new[] { 1, 2, 3 } },

            new { Nombre = "Equipos", Ruta = "/equipos",
                Icono = "router", Orden = 7,
                Roles = new[] { 1, 2, 3 } },

            new { Nombre = "Contratos", Ruta = "/contratos",
                Icono = "file-text", Orden = 8,
                Roles = new[] { 1, 2, 3 } },

            new { Nombre = "Pagos", Ruta = "/pagos",
                Icono = "credit-card", Orden = 9,
                Roles = new[] { 1, 2, 3 } }
        };

        await using var transaccion =
            await _context.Database.BeginTransactionAsync();

        foreach (var item in configuracion)
        {
            var menu = await _context.Menus
                .FirstOrDefaultAsync(x =>
                    x.Ruta == item.Ruta);

            if (menu is null)
            {
                menu = new Menu
                {
                    Nombre = item.Nombre,
                    Ruta = item.Ruta,
                    Icono = item.Icono,
                    Orden = item.Orden,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Menus.Add(menu);

                await _context.SaveChangesAsync();
            }

            foreach (var rolId in item.Roles)
            {
                var existe = await _context.RolesMenus
                    .IgnoreQueryFilters()
                    .AnyAsync(x =>
                        x.MenuId == menu.Id &&
                        x.RolId == rolId);

                if (!existe)
                {
                    _context.RolesMenus.Add(new RolMenu
                    {
                        MenuId = menu.Id,
                        RolId = rolId,
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        await transaccion.CommitAsync();
    }
}
