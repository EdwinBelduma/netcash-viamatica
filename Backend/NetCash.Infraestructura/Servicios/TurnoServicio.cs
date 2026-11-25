
using System.Data;
using Microsoft.EntityFrameworkCore;
using NetCash.Aplicacion.DTOs.Turnos;
using NetCash.Aplicacion.Interfaces;
using NetCash.Dominio.Entidades;
using NetCash.Dominio.Enumeraciones;
using NetCash.Infraestructura.Persistencia;

namespace NetCash.Infraestructura.Servicios;

public class TurnoServicio : ITurnoServicio
{
    private readonly NetCashDbContext _context;

    public TurnoServicio(NetCashDbContext context)
    {
        _context = context;
    }

    private IQueryable<Turno> Consulta()
    {
        return _context.Turnos
            .Include(x => x.Caja)
            .Include(x => x.Cajero)
            .Include(x => x.Cliente);
    }

    public async Task<List<TurnoRespuesta>> ObtenerTodosAsync(
        int? cajeroId = null)
    {
        var consulta = Consulta().AsNoTracking();

        if (cajeroId.HasValue)
        {
            consulta = consulta.Where(
                x => x.CajeroId == cajeroId.Value);
        }

        var turnos = await consulta
            .OrderByDescending(x => x.FechaTurno)
            .ToListAsync();

        return turnos.Select(Mapear).ToList();
    }

    public async Task<TurnoRespuesta?> ObtenerPorIdAsync(
        int id,
        int? cajeroId = null)
    {
        var turno = await Consulta()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                (!cajeroId.HasValue ||
                 x.CajeroId == cajeroId.Value));

        return turno is null ? null : Mapear(turno);
    }

    public async Task<TurnoRespuesta> CrearAsync(
        TurnoSolicitud solicitud,
        int gestorId)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        if (!Enum.IsDefined(solicitud.TipoAtencion))
        {
            throw new InvalidOperationException(
                "Tipo de atencion no valido.");
        }

        var caja = await _context.Cajas
            .FirstOrDefaultAsync(x =>
                x.Id == solicitud.CajaId &&
                x.Activo);

        if (caja is null)
        {
            throw new InvalidOperationException(
                "La caja seleccionada no existe.");
        }

        var cajero = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x =>
                x.Id == solicitud.CajeroId &&
                x.Activo &&
                x.Estado == EstadoUsuario.Activo);

        if (cajero is null ||
            !cajero.Rol.Activo ||
            cajero.Rol.Tipo != TipoRol.Cajero)
        {
            throw new InvalidOperationException(
                "El cajero seleccionado no es valido.");
        }

        var asignado = await _context.UsuariosCajas
            .AnyAsync(x =>
                x.CajaId == solicitud.CajaId &&
                x.UsuarioId == solicitud.CajeroId &&
                x.Activo);

        if (!asignado)
        {
            throw new InvalidOperationException(
                "El cajero no esta asignado a esta caja.");
        }

        Cliente? cliente = null;

        if (solicitud.ClienteId.HasValue)
        {
            cliente = await _context.Clientes
                .FirstOrDefaultAsync(x =>
                    x.Id == solicitud.ClienteId.Value &&
                    x.Activo);

            if (cliente is null)
            {
                throw new InvalidOperationException(
                    "El cliente no existe.");
            }
        }

        var prefijo = solicitud.TipoAtencion switch
        {
            TipoAtencion.ContratacionServicio => "AC",
            TipoAtencion.PagoServicio => "PS",
            TipoAtencion.CambioServicio => "CS",
            TipoAtencion.CambioFormaPago => "CF",
            TipoAtencion.CancelacionContrato => "CC",
            _ => throw new InvalidOperationException(
                "Tipo de atencion no valido.")
        };

        var ultimoCodigo = await _context.Turnos
            .IgnoreQueryFilters()
            .Where(x => x.Descripcion.StartsWith(prefijo))
            .OrderByDescending(x => x.Descripcion)
            .Select(x => x.Descripcion)
            .FirstOrDefaultAsync();

        var consecutivo = ultimoCodigo is null
            ? 1
            : int.Parse(ultimoCodigo[2..]) + 1;

        if (consecutivo > 9999)
        {
            throw new InvalidOperationException(
                "Se alcanzo el limite de turnos.");
        }

        var turno = new Turno
        {
            Descripcion = $"{prefijo}{consecutivo:D4}",

            CajaId = caja.Id,
            Caja = caja,

            CajeroId = cajero.Id,
            Cajero = cajero,

            GestorId = gestorId,

            ClienteId = solicitud.ClienteId,
            Cliente = cliente,

            TipoAtencion = solicitud.TipoAtencion,
            Estado = EstadoTurno.Pendiente,

            FechaTurno = DateTime.UtcNow,
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        _context.Turnos.Add(turno);

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return Mapear(turno);
    }

    public async Task<bool> IniciarAtencionAsync(
        int id,
        int cajeroId)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var turno = await _context.Turnos
            .Include(x => x.Caja)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CajeroId == cajeroId &&
                x.Activo);

        if (turno is null)
        {
            return false;
        }

        if (turno.Estado != EstadoTurno.Pendiente)
        {
            throw new InvalidOperationException(
                "El turno no se encuentra pendiente.");
        }

        if (!turno.Caja.Activo ||
            turno.Caja.UsuarioEnSesionId != cajeroId)
        {
            throw new InvalidOperationException(
                "Debe abrir la caja antes de atender.");
        }

        var tieneTurnoActivo = await _context.Turnos
            .AnyAsync(x =>
                x.CajeroId == cajeroId &&
                x.Estado == EstadoTurno.EnAtencion &&
                x.Activo);

        if (tieneTurnoActivo)
        {
            throw new InvalidOperationException(
                "Debe finalizar el turno actual.");
        }

        turno.Estado = EstadoTurno.EnAtencion;
        turno.FechaInicioAtencion = DateTime.UtcNow;
        turno.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return true;
    }

    public async Task<bool> FinalizarAtencionAsync(
        int id,
        int cajeroId)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var turno = await _context.Turnos
            .Include(x => x.Caja)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CajeroId == cajeroId &&
                x.Activo);

        if (turno is null)
        {
            return false;
        }

        if (turno.Estado != EstadoTurno.EnAtencion)
        {
            throw new InvalidOperationException(
                "El turno no se encuentra en atencion.");
        }

        if (turno.Caja.UsuarioEnSesionId != cajeroId)
        {
            throw new InvalidOperationException(
                "La caja debe estar abierta.");
        }

        if (!turno.ClienteId.HasValue)
        {
            throw new InvalidOperationException(
                "Debe asociar un cliente al turno.");
        }

        turno.Estado = EstadoTurno.Atendido;
        turno.FechaFinAtencion = DateTime.UtcNow;
        turno.FechaActualizacion = DateTime.UtcNow;

        var atencion = new Atencion
        {
            TurnoId = turno.Id,
            ClienteId = turno.ClienteId.Value,
            CajeroId = cajeroId,
            Tipo = turno.TipoAtencion,
            Estado = EstadoTurno.Atendido,
            FechaAtencion = DateTime.UtcNow,
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        _context.Atenciones.Add(atencion);

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return true;
    }

    public async Task<bool> CancelarAsync(int id)
    {
        var cantidad = await _context.Turnos
            .Where(x =>
                x.Id == id &&
                x.Activo &&
                x.Estado == EstadoTurno.Pendiente)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    x => x.Estado,
                    EstadoTurno.Cancelado)
                .SetProperty(
                    x => x.FechaActualizacion,
                    (DateTime?)DateTime.UtcNow));

        if (cantidad > 0)
        {
            return true;
        }

        if (!await _context.Turnos.AnyAsync(x => x.Id == id))
        {
            return false;
        }

        throw new InvalidOperationException(
            "Solo se pueden cancelar turnos pendientes.");
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var fecha = DateTime.UtcNow;

        var cantidad = await _context.Turnos
            .Where(x =>
                x.Id == id &&
                x.Estado == EstadoTurno.Cancelado)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Activo, false)
                .SetProperty(
                    x => x.FechaEliminacion,
                    (DateTime?)fecha)
                .SetProperty(
                    x => x.FechaActualizacion,
                    (DateTime?)fecha));

        if (cantidad > 0)
        {
            return true;
        }

        if (!await _context.Turnos.AnyAsync(x => x.Id == id))
        {
            return false;
        }

        throw new InvalidOperationException(
            "Solo se pueden eliminar turnos cancelados.");
    }

    private static TurnoRespuesta Mapear(Turno turno)
    {
        return new TurnoRespuesta
        {
            Id = turno.Id,
            Descripcion = turno.Descripcion,

            CajaId = turno.CajaId,
            Caja = turno.Caja.Descripcion,

            CajeroId = turno.CajeroId,
            Cajero = $"{turno.Cajero.Nombres} " +
                     $"{turno.Cajero.Apellidos}",

            GestorId = turno.GestorId,

            ClienteId = turno.ClienteId,

            Cliente = turno.Cliente is null
                ? null
                : $"{turno.Cliente.Nombres} " +
                  $"{turno.Cliente.Apellidos}",

            TipoAtencion = turno.TipoAtencion.ToString(),
            Estado = turno.Estado.ToString(),

            FechaTurno = turno.FechaTurno,
            FechaInicioAtencion = turno.FechaInicioAtencion,
            FechaFinAtencion = turno.FechaFinAtencion
        };
    }
}
