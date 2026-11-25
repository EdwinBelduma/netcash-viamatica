
using System.Data;
using Microsoft.EntityFrameworkCore;
using NetCash.Aplicacion.DTOs.Servicios;
using NetCash.Aplicacion.Interfaces;
using NetCash.Dominio.Entidades;
using NetCash.Dominio.Enumeraciones;
using NetCash.Infraestructura.Persistencia;

namespace NetCash.Infraestructura.Servicios;

public class CatalogoServicio : ICatalogoServicio
{
    private readonly NetCashDbContext _context;

    public CatalogoServicio(NetCashDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // SERVICIOS DE INTERNET
    // ==========================================

    public async Task<List<ServicioRespuesta>>
        ObtenerServiciosAsync()
    {
        var servicios = await _context.Servicios
            .AsNoTracking()
            .Include(x => x.Equipos)
            .Where(x => x.Activo)
            .OrderBy(x => x.Nombre)
            .ToListAsync();

        return servicios.Select(MapearServicio).ToList();
    }

    public async Task<ServicioRespuesta?>
        ObtenerServicioAsync(int id)
    {
        var servicio = await _context.Servicios
            .AsNoTracking()
            .Include(x => x.Equipos)
            .FirstOrDefaultAsync(x =>
                x.Id == id && x.Activo);

        return servicio is null
            ? null
            : MapearServicio(servicio);
    }

    public async Task<ServicioRespuesta>
        CrearServicioAsync(ServicioSolicitud solicitud)
    {
        var nombre = solicitud.Nombre.Trim();

        if (await _context.Servicios.AnyAsync(x =>
            x.Nombre.ToLower() == nombre.ToLower()))
        {
            throw new InvalidOperationException(
                "Ya existe un servicio con este nombre.");
        }

        var servicio = new Servicio
        {
            Nombre = nombre,
            Descripcion = solicitud.Descripcion.Trim(),
            VelocidadMbps = solicitud.VelocidadMbps,
            Precio = solicitud.Precio,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Servicios.Add(servicio);

        await _context.SaveChangesAsync();

        return MapearServicio(servicio);
    }

    public async Task<ServicioRespuesta?>
        ActualizarServicioAsync(
            int id,
            ServicioSolicitud solicitud)
    {
        var servicio = await _context.Servicios
            .Include(x => x.Equipos)
            .FirstOrDefaultAsync(x =>
                x.Id == id && x.Activo);

        if (servicio is null)
        {
            return null;
        }

        var nombre = solicitud.Nombre.Trim();

        if (await _context.Servicios.AnyAsync(x =>
            x.Id != id &&
            x.Nombre.ToLower() == nombre.ToLower()))
        {
            throw new InvalidOperationException(
                "El nombre pertenece a otro servicio.");
        }

        servicio.Nombre = nombre;
        servicio.Descripcion = solicitud.Descripcion.Trim();
        servicio.VelocidadMbps = solicitud.VelocidadMbps;
        servicio.Precio = solicitud.Precio;
        servicio.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapearServicio(servicio);
    }

    public async Task<bool> EliminarServicioAsync(int id)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var servicio = await _context.Servicios
            .Include(x => x.Equipos)
            .FirstOrDefaultAsync(x =>
                x.Id == id && x.Activo);

        if (servicio is null)
        {
            return false;
        }

        var tieneContratos = await _context.Contratos
            .AnyAsync(x =>
                x.ServicioId == id &&
                x.Activo &&
                (x.Estado == EstadoContrato.Vigente ||
                 x.Estado == EstadoContrato.Renovado));

        if (tieneContratos)
        {
            throw new InvalidOperationException(
                "El servicio tiene contratos vigentes.");
        }

        foreach (var equipo in servicio.Equipos
            .Where(x => x.Activo))
        {
            equipo.Eliminar();
        }

        servicio.Eliminar();

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return true;
    }

    // ==========================================
    // EQUIPOS
    // ==========================================

    public async Task<List<EquipoRespuesta>>
        ObtenerEquiposAsync(int? servicioId = null)
    {
        var consulta = _context.Equipos
            .AsNoTracking()
            .Where(x => x.Activo);

        if (servicioId.HasValue)
        {
            consulta = consulta.Where(x =>
                x.ServicioId == servicioId.Value);
        }

        var equipos = await consulta
            .OrderBy(x => x.Nombre)
            .ToListAsync();

        return equipos.Select(MapearEquipo).ToList();
    }

    public async Task<EquipoRespuesta?>
        ObtenerEquipoAsync(int id)
    {
        var equipo = await _context.Equipos
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == id && x.Activo);

        return equipo is null
            ? null
            : MapearEquipo(equipo);
    }

    public async Task<EquipoRespuesta>
        CrearEquipoAsync(EquipoSolicitud solicitud)
    {
        await ValidarServicioAsync(solicitud.ServicioId);

        var equipo = new Equipo
        {
            Nombre = solicitud.Nombre.Trim(),
            Descripcion = solicitud.Descripcion.Trim(),
            ServicioId = solicitud.ServicioId,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Equipos.Add(equipo);

        await _context.SaveChangesAsync();

        return MapearEquipo(equipo);
    }

    public async Task<EquipoRespuesta?>
        ActualizarEquipoAsync(
            int id,
            EquipoSolicitud solicitud)
    {
        var equipo = await _context.Equipos
            .FirstOrDefaultAsync(x =>
                x.Id == id && x.Activo);

        if (equipo is null)
        {
            return null;
        }

        await ValidarServicioAsync(solicitud.ServicioId);

        equipo.Nombre = solicitud.Nombre.Trim();
        equipo.Descripcion = solicitud.Descripcion.Trim();
        equipo.ServicioId = solicitud.ServicioId;
        equipo.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapearEquipo(equipo);
    }

    public async Task<bool> EliminarEquipoAsync(int id)
    {
        var equipo = await _context.Equipos
            .FirstOrDefaultAsync(x =>
                x.Id == id && x.Activo);

        if (equipo is null)
        {
            return false;
        }

        equipo.Eliminar();

        await _context.SaveChangesAsync();

        return true;
    }

    private async Task ValidarServicioAsync(int servicioId)
    {
        var existe = await _context.Servicios
            .AnyAsync(x =>
                x.Id == servicioId && x.Activo);

        if (!existe)
        {
            throw new InvalidOperationException(
                "El servicio seleccionado no existe.");
        }
    }

    // ==========================================
    // MAPEO DE DTOs
    // ==========================================

    private static ServicioRespuesta MapearServicio(
        Servicio servicio)
    {
        return new ServicioRespuesta
        {
            Id = servicio.Id,
            Nombre = servicio.Nombre,
            Descripcion = servicio.Descripcion,
            VelocidadMbps = servicio.VelocidadMbps,
            Precio = servicio.Precio,
            Activo = servicio.Activo,

            Equipos = servicio.Equipos
                .Where(x => x.Activo)
                .Select(MapearEquipo)
                .ToList()
        };
    }

    private static EquipoRespuesta MapearEquipo(
        Equipo equipo)
    {
        return new EquipoRespuesta
        {
            Id = equipo.Id,
            Nombre = equipo.Nombre,
            Descripcion = equipo.Descripcion,
            ServicioId = equipo.ServicioId,
            Activo = equipo.Activo
        };
    }
}
