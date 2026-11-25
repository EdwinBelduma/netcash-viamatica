
using System.Data;
using Microsoft.EntityFrameworkCore;
using NetCash.Aplicacion.DTOs.Contratos;
using NetCash.Aplicacion.DTOs.Pagos;
using NetCash.Aplicacion.Interfaces;
using NetCash.Dominio.Entidades;
using NetCash.Dominio.Enumeraciones;
using NetCash.Infraestructura.Persistencia;

namespace NetCash.Infraestructura.Servicios;

public class OperacionServicio : IOperacionServicio
{
    private readonly NetCashDbContext _context;

    public OperacionServicio(NetCashDbContext context)
    {
        _context = context;
    }

    // CONSULTAR CONTRATOS

    public async Task<List<ContratoRespuesta>>
        ObtenerContratosAsync(int? clienteId = null)
    {
        var consulta = _context.Contratos.AsNoTracking();

        if (clienteId.HasValue)
        {
            consulta = consulta.Where(x =>
                x.ClienteId == clienteId.Value);
        }

        var contratos = await consulta
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return contratos.Select(MapearContrato).ToList();
    }

    public async Task<ContratoRespuesta?>
        ObtenerContratoAsync(int id)
    {
        var contrato = await _context.Contratos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return contrato is null
            ? null
            : MapearContrato(contrato);
    }

    // CREAR CONTRATO

    public async Task<ContratoRespuesta> CrearContratoAsync(
        ContratoSolicitud solicitud,
        int cajeroId)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var fecha = DateTime.UtcNow;

        if (solicitud.FechaFin.UtcDateTime <= fecha)
        {
            throw new InvalidOperationException(
                "La fecha de finalizacion debe ser futura.");
        }

        var turno = await ValidarTurnoAsync(
            solicitud.TurnoId,
            cajeroId,
            solicitud.ClienteId,
            TipoAtencion.ContratacionServicio);

        var servicio = await ValidarServicioAsync(
            solicitud.ServicioId);

        await ValidarFormaPagoAsync(
            solicitud.FormaPagoId);

        var contrato = new Contrato
        {
            ClienteId = solicitud.ClienteId,
            ServicioId = servicio.Id,
            FormaPagoId = solicitud.FormaPagoId,
            CajeroId = cajeroId,
            FechaInicio = fecha,
            FechaFin = solicitud.FechaFin.UtcDateTime,
            Estado = EstadoContrato.Vigente,
            PrecioContratado = servicio.Precio,
            FechaCreacion = fecha,
            Activo = true
        };

        _context.Contratos.Add(contrato);

        CompletarTurno(turno, solicitud.ClienteId);

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return MapearContrato(contrato);
    }

    // CAMBIAR SERVICIO

    public async Task<ContratoRespuesta>
        CambiarServicioAsync(
            int id,
            CambioServicioSolicitud solicitud,
            int cajeroId)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var anterior = await ObtenerVigenteAsync(id);

        var turno = await ValidarTurnoAsync(
            solicitud.TurnoId,
            cajeroId,
            anterior.ClienteId,
            TipoAtencion.CambioServicio);

        if (anterior.ServicioId == solicitud.ServicioId)
        {
            throw new InvalidOperationException(
                "Seleccione un servicio diferente.");
        }

        var servicio = await ValidarServicioAsync(
            solicitud.ServicioId);

        var fecha = DateTime.UtcNow;

        anterior.Estado = EstadoContrato.Sustituido;
        anterior.FechaActualizacion = fecha;

        var nuevo = new Contrato
        {
            ClienteId = anterior.ClienteId,
            ServicioId = servicio.Id,
            FormaPagoId = anterior.FormaPagoId,
            CajeroId = cajeroId,
            FechaInicio = fecha,
            FechaFin = anterior.FechaFin,
            Estado = EstadoContrato.Renovado,
            PrecioContratado = servicio.Precio,
            ContratoAnteriorId = anterior.Id,
            FechaCreacion = fecha,
            Activo = true
        };

        _context.Contratos.Add(nuevo);

        CompletarTurno(turno, anterior.ClienteId);

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return MapearContrato(nuevo);
    }

    // CAMBIAR FORMA DE PAGO

    public async Task<ContratoRespuesta>
        CambiarFormaPagoAsync(
            int id,
            CambioFormaPagoSolicitud solicitud,
            int cajeroId)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var contrato = await ObtenerVigenteAsync(id);

        var turno = await ValidarTurnoAsync(
            solicitud.TurnoId,
            cajeroId,
            contrato.ClienteId,
            TipoAtencion.CambioFormaPago);

        await ValidarFormaPagoAsync(
            solicitud.FormaPagoId);

        if (contrato.FormaPagoId == solicitud.FormaPagoId)
        {
            throw new InvalidOperationException(
                "Seleccione una forma de pago diferente.");
        }

        contrato.FormaPagoId = solicitud.FormaPagoId;
        contrato.FechaActualizacion = DateTime.UtcNow;

        CompletarTurno(turno, contrato.ClienteId);

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return MapearContrato(contrato);
    }

    // CANCELAR CONTRATO

    public async Task<ContratoRespuesta>
        CancelarContratoAsync(
            int id,
            CancelacionContratoSolicitud solicitud,
            int cajeroId)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var contrato = await ObtenerVigenteAsync(id);

        var turno = await ValidarTurnoAsync(
            solicitud.TurnoId,
            cajeroId,
            contrato.ClienteId,
            TipoAtencion.CancelacionContrato);

        var fecha = DateTime.UtcNow;

        contrato.Estado = EstadoContrato.Cancelado;
        contrato.FechaFin = fecha;

        contrato.MotivoCancelacion =
            solicitud.Motivo.Trim();

        contrato.FechaActualizacion = fecha;

        CompletarTurno(turno, contrato.ClienteId);

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return MapearContrato(contrato);
    }

    // CONSULTAR PAGOS

    public async Task<List<PagoRespuesta>>
        ObtenerPagosAsync(
            int? contratoId = null,
            int? cajeroId = null)
    {
        var consulta = _context.Pagos.AsNoTracking();

        if (contratoId.HasValue)
        {
            consulta = consulta.Where(x =>
                x.ContratoId == contratoId.Value);
        }

        if (cajeroId.HasValue)
        {
            consulta = consulta.Where(x =>
                x.CajeroId == cajeroId.Value);
        }

        var pagos = await consulta
            .OrderByDescending(x => x.FechaPago)
            .ToListAsync();

        return pagos.Select(MapearPago).ToList();
    }

    // REGISTRAR PAGO

    public async Task<PagoRespuesta> RegistrarPagoAsync(
        PagoSolicitud solicitud,
        int cajeroId)
    {
        await using var transaccion =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var contrato = await ObtenerVigenteAsync(
            solicitud.ContratoId);

        var turno = await ValidarTurnoAsync(
            solicitud.TurnoId,
            cajeroId,
            contrato.ClienteId,
            TipoAtencion.PagoServicio);

        await ValidarFormaPagoAsync(
            solicitud.FormaPagoId);

        if (solicitud.Monto <= 0 ||
            decimal.Round(solicitud.Monto, 2)
                != solicitud.Monto)
        {
            throw new InvalidOperationException(
                "El monto debe ser positivo y tener maximo dos decimales.");
        }

        var pago = new Pago
        {
            ClienteId = contrato.ClienteId,
            ContratoId = contrato.Id,
            CajaId = turno.CajaId,
            CajeroId = cajeroId,
            FormaPagoId = solicitud.FormaPagoId,
            Monto = solicitud.Monto,
            Referencia = solicitud.Referencia?.Trim(),
            Observacion = solicitud.Observacion?.Trim(),
            FechaPago = DateTime.UtcNow,
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        _context.Pagos.Add(pago);

        CompletarTurno(turno, contrato.ClienteId);

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return MapearPago(pago);
    }

    // VALIDAR CONTRATO VIGENTE

    private async Task<Contrato> ObtenerVigenteAsync(
        int id)
    {
        var contrato = await _context.Contratos
            .FirstOrDefaultAsync(x =>
                x.Id == id && x.Activo);

        if (contrato is null)
        {
            throw new InvalidOperationException(
                "El contrato no existe.");
        }

        if (contrato.Estado != EstadoContrato.Vigente &&
            contrato.Estado != EstadoContrato.Renovado)
        {
            throw new InvalidOperationException(
                "El contrato no se encuentra vigente.");
        }

        if (contrato.FechaFin <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "El contrato ha expirado.");
        }

        return contrato;
    }

    // VALIDAR SERVICIO

    private async Task<Servicio> ValidarServicioAsync(
        int id)
    {
        var servicio = await _context.Servicios
            .FirstOrDefaultAsync(x =>
                x.Id == id && x.Activo);

        return servicio ??
            throw new InvalidOperationException(
                "El servicio no existe.");
    }

    // VALIDAR FORMA DE PAGO

    private async Task ValidarFormaPagoAsync(int id)
    {
        var existe = await _context.FormasPago
            .AnyAsync(x => x.Id == id && x.Activo);

        if (!existe)
        {
            throw new InvalidOperationException(
                "La forma de pago no existe.");
        }
    }

    // VALIDAR TURNO Y CAJERO

    private async Task<Turno> ValidarTurnoAsync(
        int turnoId,
        int cajeroId,
        int clienteId,
        TipoAtencion tipo)
    {
        var turno = await _context.Turnos
            .Include(x => x.Caja)
            .FirstOrDefaultAsync(x =>
                x.Id == turnoId &&
                x.CajeroId == cajeroId &&
                x.Activo);

        if (turno is null)
        {
            throw new InvalidOperationException(
                "El turno no pertenece al cajero.");
        }

        if (turno.Estado != EstadoTurno.EnAtencion)
        {
            throw new InvalidOperationException(
                "Debe iniciar la atencion del turno.");
        }

        if (turno.TipoAtencion != tipo)
        {
            throw new InvalidOperationException(
                "El turno no corresponde a la operacion.");
        }

        if (!turno.Caja.Activo ||
            turno.Caja.UsuarioEnSesionId != cajeroId)
        {
            throw new InvalidOperationException(
                "Debe tener abierta la caja.");
        }

        var asignado = await _context.UsuariosCajas
            .AnyAsync(x =>
                x.CajaId == turno.CajaId &&
                x.UsuarioId == cajeroId &&
                x.Activo);

        if (!asignado)
        {
            throw new InvalidOperationException(
                "El cajero no esta asignado a esta caja.");
        }

        if (turno.ClienteId.HasValue &&
            turno.ClienteId.Value != clienteId)
        {
            throw new InvalidOperationException(
                "El cliente no corresponde al turno.");
        }

        var clienteExiste = await _context.Clientes
            .AnyAsync(x =>
                x.Id == clienteId && x.Activo);

        if (!clienteExiste)
        {
            throw new InvalidOperationException(
                "El cliente no existe.");
        }

        turno.ClienteId = clienteId;

        return turno;
    }

    // COMPLETAR ATENCION

    private void CompletarTurno(
        Turno turno,
        int clienteId)
    {
        var fecha = DateTime.UtcNow;

        turno.ClienteId = clienteId;
        turno.Estado = EstadoTurno.Atendido;
        turno.FechaFinAtencion = fecha;
        turno.FechaActualizacion = fecha;

        var atencion = new Atencion
        {
            TurnoId = turno.Id,
            ClienteId = clienteId,
            CajeroId = turno.CajeroId,
            Tipo = turno.TipoAtencion,
            Estado = EstadoTurno.Atendido,
            FechaAtencion = fecha,
            FechaCreacion = fecha,
            Activo = true
        };

        _context.Atenciones.Add(atencion);
    }

    // MAPEAR CONTRATO

    private static ContratoRespuesta MapearContrato(
        Contrato contrato)
    {
        return new ContratoRespuesta
        {
            Id = contrato.Id,
            ClienteId = contrato.ClienteId,
            ServicioId = contrato.ServicioId,
            FormaPagoId = contrato.FormaPagoId,
            CajeroId = contrato.CajeroId,
            FechaInicio = contrato.FechaInicio,
            FechaFin = contrato.FechaFin,
            PrecioContratado = contrato.PrecioContratado,
            Estado = contrato.Estado.ToString(),
            ContratoAnteriorId = contrato.ContratoAnteriorId,
            MotivoCancelacion = contrato.MotivoCancelacion,
            Activo = contrato.Activo
        };
    }

    // MAPEAR PAGO

    private static PagoRespuesta MapearPago(Pago pago)
    {
        return new PagoRespuesta
        {
            Id = pago.Id,
            ClienteId = pago.ClienteId,
            ContratoId = pago.ContratoId,
            CajaId = pago.CajaId,
            CajeroId = pago.CajeroId,
            FormaPagoId = pago.FormaPagoId,
            Monto = pago.Monto,
            FechaPago = pago.FechaPago,
            Referencia = pago.Referencia,
            Observacion = pago.Observacion
        };
    }
}
