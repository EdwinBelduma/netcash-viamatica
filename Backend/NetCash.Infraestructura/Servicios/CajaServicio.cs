
using System.Data;
using Microsoft.EntityFrameworkCore;
using NetCash.Aplicacion.DTOs.Cajas;
using NetCash.Aplicacion.Interfaces;
using NetCash.Dominio.Entidades;
using NetCash.Dominio.Enumeraciones;
using NetCash.Infraestructura.Persistencia;

namespace NetCash.Infraestructura.Servicios;

public class CajaServicio : ICajaServicio
{
    private readonly NetCashDbContext _context;

    public CajaServicio(NetCashDbContext context)
    {
        _context = context;
    }

    public async Task<List<CajaRespuesta>> ObtenerTodosAsync()
    {
        var cajas = await _context.Cajas
            .AsNoTracking()
            .Include(x => x.UsuariosCaja)
            .OrderBy(x => x.Id)
            .ToListAsync();

        return cajas.Select(Mapear).ToList();
    }

    public async Task<CajaRespuesta?> ObtenerPorIdAsync(int id)
    {
        var caja = await _context.Cajas
            .Include(x => x.UsuariosCaja)
            .FirstOrDefaultAsync(x => x.Id == id);

        return caja is null ? null : Mapear(caja);
    }

    public async Task<CajaRespuesta> CrearAsync(
        CajaSolicitud solicitud)
    {
        var descripcion = solicitud.Descripcion.Trim();

        if (await _context.Cajas.AnyAsync(x =>
            x.Descripcion.ToLower() == descripcion.ToLower()))
        {
            throw new InvalidOperationException(
                "Ya existe una caja con esta descripcion.");
        }

        var caja = new Caja
        {
            Descripcion = descripcion,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Cajas.Add(caja);

        await _context.SaveChangesAsync();

        return Mapear(caja);
    }

    public async Task<CajaRespuesta?> ActualizarAsync(
        int id,
        CajaSolicitud solicitud)
    {
        var caja = await _context.Cajas
            .Include(x => x.UsuariosCaja)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (caja is null)
        {
            return null;
        }

        var descripcion = solicitud.Descripcion.Trim();

        if (await _context.Cajas.AnyAsync(x =>
            x.Id != id &&
            x.Descripcion.ToLower() == descripcion.ToLower()))
        {
            throw new InvalidOperationException(
                "La descripcion pertenece a otra caja.");
        }

        caja.Descripcion = descripcion;
        caja.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Mapear(caja);
    }

    public async Task<bool> EliminarAsync(int id)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var caja = await _context.Cajas
            .Include(x => x.UsuariosCaja)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (caja is null)
        {
            return false;
        }

        if (caja.UsuarioEnSesionId.HasValue)
        {
            throw new InvalidOperationException(
                "No se puede eliminar una caja ocupada.");
        }

        if (caja.UsuariosCaja.Any(x => x.Activo))
        {
            throw new InvalidOperationException(
                "La caja tiene cajeros asignados.");
        }

        if (await _context.Turnos.AnyAsync(x =>
            x.CajaId == id &&
            x.Activo &&
            (x.Estado == EstadoTurno.Pendiente ||
             x.Estado == EstadoTurno.EnAtencion)))
        {
            throw new InvalidOperationException(
                "La caja tiene turnos pendientes.");
        }

        caja.Eliminar();

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return true;
    }

    public async Task<bool> AsignarCajeroAsync(
        int cajaId,
        int usuarioId,
        int gestorId)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var caja = await _context.Cajas
            .FirstOrDefaultAsync(x =>
                x.Id == cajaId && x.Activo);

        if (caja is null)
        {
            return false;
        }

        var usuario = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x =>
                x.Id == usuarioId &&
                x.Activo &&
                x.Estado == EstadoUsuario.Activo);

        if (usuario is null ||
            !usuario.Rol.Activo ||
            usuario.Rol.Tipo != TipoRol.Cajero)
        {
            throw new InvalidOperationException(
                "El usuario debe ser un cajero activo.");
        }

        var existe = await _context.UsuariosCajas
            .AnyAsync(x =>
                x.CajaId == cajaId &&
                x.UsuarioId == usuarioId &&
                x.Activo);

        if (existe)
        {
            throw new InvalidOperationException(
                "El cajero ya esta asignado a esta caja.");
        }

        var cantidad = await _context.UsuariosCajas
            .CountAsync(x =>
                x.CajaId == cajaId && x.Activo);

        if (cantidad >= 2)
        {
            throw new InvalidOperationException(
                "La caja admite un maximo de dos cajeros.");
        }

        var asignacion = new UsuarioCaja
        {
            CajaId = cajaId,
            UsuarioId = usuarioId,
            GestorId = gestorId,
            FechaAsignacion = DateTime.UtcNow,
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        _context.UsuariosCajas.Add(asignacion);

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return true;
    }

    public async Task<bool> DesasignarCajeroAsync(
        int cajaId,
        int usuarioId)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var caja = await _context.Cajas
            .FirstOrDefaultAsync(x => x.Id == cajaId);

        if (caja is null)
        {
            return false;
        }

        if (caja.UsuarioEnSesionId == usuarioId)
        {
            throw new InvalidOperationException(
                "El cajero debe cerrar la caja primero.");
        }

        var asignacion = await _context.UsuariosCajas
            .FirstOrDefaultAsync(x =>
                x.CajaId == cajaId &&
                x.UsuarioId == usuarioId &&
                x.Activo);

        if (asignacion is null)
        {
            return false;
        }

        asignacion.Eliminar();

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return true;
    }

    public async Task<bool> AbrirCajaAsync(
        int cajaId,
        int usuarioId)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var asignado = await _context.UsuariosCajas
            .AnyAsync(x =>
                x.CajaId == cajaId &&
                x.UsuarioId == usuarioId &&
                x.Activo &&
                x.Usuario.Activo &&
                x.Usuario.Estado == EstadoUsuario.Activo);

        if (!asignado)
        {
            throw new InvalidOperationException(
                "El cajero no tiene asignada esta caja.");
        }

        var otraCaja = await _context.Cajas
            .AnyAsync(x =>
                x.UsuarioEnSesionId == usuarioId);

        if (otraCaja)
        {
            throw new InvalidOperationException(
                "El cajero ya tiene una caja abierta.");
        }

        var fecha = DateTime.UtcNow;

        var actualizadas = await _context.Cajas
            .Where(x =>
                x.Id == cajaId &&
                x.Activo &&
                x.UsuarioEnSesionId == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    x => x.UsuarioEnSesionId,
                    (int?)usuarioId)
                .SetProperty(
                    x => x.FechaApertura,
                    (DateTime?)fecha)
                .SetProperty(
                    x => x.FechaActualizacion,
                    (DateTime?)fecha));

        if (actualizadas == 0)
        {
            throw new InvalidOperationException(
                "La caja no existe o se encuentra ocupada.");
        }

        await transaccion.CommitAsync();

        return true;
    }

    public async Task<bool> CerrarCajaAsync(
        int cajaId,
        int usuarioId)
    {
        var fecha = DateTime.UtcNow;

        var actualizadas = await _context.Cajas
            .Where(x =>
                x.Id == cajaId &&
                x.UsuarioEnSesionId == usuarioId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    x => x.UsuarioEnSesionId,
                    (int?)null)
                .SetProperty(
                    x => x.FechaApertura,
                    (DateTime?)null)
                .SetProperty(
                    x => x.FechaActualizacion,
                    (DateTime?)fecha));

        return actualizadas > 0;
    }

    private static CajaRespuesta Mapear(Caja caja)
    {
        var cajeros = caja.UsuariosCaja
            .Where(x => x.Activo)
            .Select(x => x.UsuarioId)
            .ToList();

        return new CajaRespuesta
        {
            Id = caja.Id,
            Descripcion = caja.Descripcion,
            Activo = caja.Activo,
            Ocupada = caja.UsuarioEnSesionId.HasValue,
            UsuarioEnSesionId = caja.UsuarioEnSesionId,
            FechaApertura = caja.FechaApertura,
            CantidadCajeros = cajeros.Count,
            CajerosAsignados = cajeros
        };
    }
}
