using Microsoft.EntityFrameworkCore;
using NetCash.Aplicacion.DTOs.Dashboard;
using NetCash.Aplicacion.DTOs.Menus;
using NetCash.Aplicacion.Interfaces;
using NetCash.Dominio.Enumeraciones;
using NetCash.Infraestructura.Persistencia;

namespace NetCash.Infraestructura.Servicios;

public class DashboardServicio : IDashboardServicio
{
    private readonly NetCashDbContext _context;

    public DashboardServicio(NetCashDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // RESUMEN GENERAL DEL DASHBOARD
    // ==========================================

    public async Task<DashboardRespuesta> ObtenerResumenAsync(
        int usuarioId,
        string rol,
        DateTime desde,
        DateTime hasta)
    {
        // ==========================================
        // CONSULTAS BASE
        // ==========================================

        var turnos = _context.Turnos
            .AsNoTracking()
            .Where(x =>
                x.Activo &&
                x.FechaTurno >= desde &&
                x.FechaTurno < hasta);

        var atendidos = _context.Turnos
            .AsNoTracking()
            .Where(x =>
                x.Activo &&
                x.Estado == EstadoTurno.Atendido &&
                x.FechaFinAtencion >= desde &&
                x.FechaFinAtencion < hasta);

        var pagos = _context.Pagos
            .AsNoTracking()
            .Where(x =>
                x.Activo &&
                x.FechaPago >= desde &&
                x.FechaPago < hasta);

        var cajas = _context.Cajas
            .AsNoTracking()
            .Where(x => x.Activo);

        var contratos = _context.Contratos
            .AsNoTracking()
            .Where(x =>
                x.Activo &&
                x.FechaFin > DateTime.UtcNow &&
                (x.Estado == EstadoContrato.Vigente ||
                 x.Estado == EstadoContrato.Renovado));

        // ==========================================
        // FILTROS POR ROL
        // ==========================================

        if (rol == "Cajero")
        {
            turnos = turnos.Where(x =>
                x.CajeroId == usuarioId);

            atendidos = atendidos.Where(x =>
                x.CajeroId == usuarioId);

            pagos = pagos.Where(x =>
                x.CajeroId == usuarioId);

            cajas = cajas.Where(x =>
                x.UsuariosCaja.Any(a =>
                    a.UsuarioId == usuarioId &&
                    a.Activo));

            contratos = contratos.Where(x =>
                x.CajeroId == usuarioId);
        }
        else if (rol == "Gestor")
        {
            turnos = turnos.Where(x =>
                x.GestorId == usuarioId);

            atendidos = atendidos.Where(x =>
                x.GestorId == usuarioId);

            cajas = cajas.Where(x =>
                x.UsuariosCaja.Any(a =>
                    a.GestorId == usuarioId &&
                    a.Activo));

            pagos = pagos.Where(x =>
                cajas.Any(c => c.Id == x.CajaId));

            contratos = contratos.Where(x =>
                _context.UsuariosCajas.Any(a =>
                    a.GestorId == usuarioId &&
                    a.UsuarioId == x.CajeroId &&
                    a.Activo));
        }
        else if (rol != "Administrador")
        {
            throw new UnauthorizedAccessException(
                "El usuario no tiene permisos para consultar el dashboard.");
        }

        // ==========================================
        // FUNCION ALMACENADA DE POSTGRESQL
        // ==========================================

        int totalTurnosAtendidos;

        if (rol == "Administrador")
        {
            totalTurnosAtendidos =
                await ObtenerTurnosAtendidosSqlAsync(
                    desde,
                    hasta,
                    null);
        }
        else if (rol == "Cajero")
        {
            totalTurnosAtendidos =
                await ObtenerTurnosAtendidosSqlAsync(
                    desde,
                    hasta,
                    usuarioId);
        }
        else
        {
            // La funcion SQL actual filtra por cajero,
            // no por gestor. Se conserva el filtro
            // correspondiente mediante Entity Framework.

            totalTurnosAtendidos =
                await atendidos.CountAsync();
        }

        // ==========================================
        // INDICADORES GENERALES
        // ==========================================

        var respuesta = new DashboardRespuesta
        {
            TotalCajas = await cajas.CountAsync(),

            CajasOcupadas = await cajas.CountAsync(x =>
                x.UsuarioEnSesionId != null),

            CajasDisponibles = await cajas.CountAsync(x =>
                x.UsuarioEnSesionId == null),

            TurnosPendientes = await turnos.CountAsync(x =>
                x.Estado == EstadoTurno.Pendiente),

            TurnosEnAtencion = await turnos.CountAsync(x =>
                x.Estado == EstadoTurno.EnAtencion),

            TurnosAtendidos = totalTurnosAtendidos,

            ContratosVigentes = await contratos.CountAsync(),

            TotalPagos = await pagos.CountAsync(),

            TotalRecaudado = await pagos
                .SumAsync(x => (decimal?)x.Monto) ?? 0
        };

        // ==========================================
        // INDICADORES DEL ADMINISTRADOR
        // ==========================================

        if (rol == "Administrador")
        {
            respuesta.TotalClientes = await _context.Clientes
                .CountAsync(x => x.Activo);

            respuesta.TotalUsuarios = await _context.Usuarios
                .CountAsync(x => x.Activo);

            respuesta.CajerosActivos = await _context.Usuarios
                .CountAsync(x =>
                    x.Activo &&
                    x.Estado == EstadoUsuario.Activo &&
                    x.Rol.Tipo == TipoRol.Cajero);

            respuesta.GestoresActivos = await _context.Usuarios
                .CountAsync(x =>
                    x.Activo &&
                    x.Estado == EstadoUsuario.Activo &&
                    x.Rol.Tipo == TipoRol.Gestor);

            respuesta.UsuariosPendientes = await _context.Usuarios
                .CountAsync(x =>
                    x.Activo &&
                    x.Estado == EstadoUsuario.PendienteAprobacion);

            respuesta.SesionesActivas = await _context.Usuarios
                .CountAsync(x =>
                    x.Activo &&
                    x.SesionActiva);

            respuesta.Sesiones = await _context.Usuarios
                .AsNoTracking()
                .Select(x => new SesionRespuesta
                {
                    Id = x.Id,
                    NombreUsuario = x.NombreUsuario,
                    Rol = x.Rol.Tipo.ToString(),
                    SesionActiva = x.SesionActiva,
                    UltimoInicioSesion = x.UltimoInicioSesion
                })
                .ToListAsync();
        }

        // ==========================================
        // INDICADORES DEL GESTOR
        // ==========================================

        if (rol == "Gestor")
        {
            respuesta.UsuariosPendientes =
                await _context.Usuarios
                    .CountAsync(x =>
                        x.Activo &&
                        x.UsuarioCreacionId == usuarioId &&
                        x.Estado ==
                            EstadoUsuario.PendienteAprobacion);
        }

        // ==========================================
        // INDICADORES POR CAJA
        // ==========================================

        respuesta.IndicadoresCajas = await atendidos
            .GroupBy(x => new
            {
                x.CajaId,
                x.Caja.Descripcion
            })
            .Select(g => new IndicadorRespuesta
            {
                Id = g.Key.CajaId,
                Nombre = g.Key.Descripcion,
                TurnosAtendidos = g.Count()
            })
            .OrderByDescending(x => x.TurnosAtendidos)
            .ToListAsync();

        // ==========================================
        // INDICADORES POR CAJERO
        // ==========================================

        respuesta.IndicadoresCajeros = await atendidos
            .GroupBy(x => new
            {
                x.CajeroId,
                x.Cajero.Nombres,
                x.Cajero.Apellidos
            })
            .Select(g => new IndicadorRespuesta
            {
                Id = g.Key.CajeroId,
                Nombre = g.Key.Nombres + " " +
                         g.Key.Apellidos,
                TurnosAtendidos = g.Count()
            })
            .OrderByDescending(x => x.TurnosAtendidos)
            .ToListAsync();

        // ==========================================
        // INDICADORES POR GESTOR
        // ==========================================

        if (rol == "Administrador" || rol == "Gestor")
        {
            respuesta.IndicadoresGestores = await atendidos
                .GroupBy(x => new
                {
                    x.GestorId,
                    x.Gestor.Nombres,
                    x.Gestor.Apellidos
                })
                .Select(g => new IndicadorRespuesta
                {
                    Id = g.Key.GestorId,
                    Nombre = g.Key.Nombres + " " +
                             g.Key.Apellidos,
                    TurnosAtendidos = g.Count()
                })
                .OrderByDescending(x => x.TurnosAtendidos)
                .ToListAsync();
        }

        return respuesta;
    }

    // ==========================================
    // EJECUTAR FUNCION SQL DE POSTGRESQL
    // ==========================================

    private async Task<int> ObtenerTurnosAtendidosSqlAsync(
        DateTime desde,
        DateTime hasta,
        int? cajeroId)
    {
        var total = await _context.Database
            .SqlQuery<long>(
                $"""
                SELECT fn_turnos_atendidos(
                    {desde},
                    {hasta},
                    CAST({cajeroId} AS INTEGER)
                ) AS "Value"
                """)
            .SingleAsync();

        return checked((int)total);
    }

    // ==========================================
    // MENU DINAMICO SEGUN ROL
    // ==========================================

    public async Task<List<MenuRespuesta>> ObtenerMenuAsync(
        int usuarioId)
    {
        var usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == usuarioId &&
                x.Activo &&
                x.Estado == EstadoUsuario.Activo);

        if (usuario is null)
        {
            return new List<MenuRespuesta>();
        }

        return await _context.RolesMenus
            .AsNoTracking()
            .Where(x =>
                x.RolId == usuario.RolId &&
                x.Activo &&
                x.Menu.Activo)
            .OrderBy(x => x.Menu.Orden)
            .Select(x => new MenuRespuesta
            {
                Id = x.Menu.Id,
                Nombre = x.Menu.Nombre,
                Ruta = x.Menu.Ruta,
                Icono = x.Menu.Icono,
                Orden = x.Menu.Orden,
                MenuPadreId = x.Menu.MenuPadreId
            })
            .ToListAsync();
    }
}