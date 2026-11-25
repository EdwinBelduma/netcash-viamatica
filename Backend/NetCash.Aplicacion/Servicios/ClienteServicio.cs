
using NetCash.Aplicacion.DTOs.Clientes;
using NetCash.Aplicacion.Interfaces;
using NetCash.Dominio.Entidades;

namespace NetCash.Aplicacion.Servicios;

public class ClienteServicio : IClienteServicio
{
    private readonly IClienteRepositorio _repositorio;

    public ClienteServicio(IClienteRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<ClienteRespuesta>>
        ObtenerTodosAsync()
    {
        var clientes = await _repositorio.ObtenerTodosAsync();

        return clientes.Select(Mapear).ToList();
    }

    public async Task<ClienteRespuesta?>
        ObtenerPorIdAsync(int id)
    {
        var cliente = await _repositorio.ObtenerPorIdAsync(id);

        return cliente is null ? null : Mapear(cliente);
    }

    public async Task<ClienteRespuesta> CrearAsync(
        ClienteSolicitud solicitud)
    {
        var identificacion = solicitud.Identificacion.Trim();

        if (await _repositorio
            .ExisteIdentificacionAsync(identificacion))
        {
            throw new InvalidOperationException(
                "Ya existe un cliente con esta identificacion.");
        }

        var cliente = new Cliente
        {
            Nombres = solicitud.Nombres.Trim(),
            Apellidos = solicitud.Apellidos.Trim(),
            Identificacion = identificacion,
            Correo = solicitud.Correo.Trim(),
            Telefono = solicitud.Telefono.Trim(),
            Direccion = solicitud.Direccion.Trim(),
            ReferenciaDireccion =
                solicitud.ReferenciaDireccion.Trim(),

            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        await _repositorio.AgregarAsync(cliente);
        await _repositorio.GuardarCambiosAsync();

        return Mapear(cliente);
    }

    public async Task<ClienteRespuesta?> ActualizarAsync(
        int id,
        ClienteSolicitud solicitud)
    {
        var cliente = await _repositorio.ObtenerPorIdAsync(id);

        if (cliente is null)
        {
            return null;
        }

        if (await _repositorio.ExisteIdentificacionAsync(
            solicitud.Identificacion.Trim(), id))
        {
            throw new InvalidOperationException(
                "La identificacion pertenece a otro cliente.");
        }

        cliente.Nombres = solicitud.Nombres.Trim();
        cliente.Apellidos = solicitud.Apellidos.Trim();
        cliente.Identificacion = solicitud.Identificacion.Trim();
        cliente.Correo = solicitud.Correo.Trim();
        cliente.Telefono = solicitud.Telefono.Trim();
        cliente.Direccion = solicitud.Direccion.Trim();

        cliente.ReferenciaDireccion =
            solicitud.ReferenciaDireccion.Trim();

        cliente.FechaActualizacion = DateTime.UtcNow;

        await _repositorio.GuardarCambiosAsync();

        return Mapear(cliente);
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var cliente = await _repositorio.ObtenerPorIdAsync(id);

        if (cliente is null)
        {
            return false;
        }

        cliente.Eliminar();

        await _repositorio.GuardarCambiosAsync();

        return true;
    }

    private static ClienteRespuesta Mapear(Cliente cliente)
    {
        return new ClienteRespuesta
        {
            Id = cliente.Id,
            Nombres = cliente.Nombres,
            Apellidos = cliente.Apellidos,
            Identificacion = cliente.Identificacion,
            Correo = cliente.Correo,
            Telefono = cliente.Telefono,
            Direccion = cliente.Direccion,
            ReferenciaDireccion = cliente.ReferenciaDireccion,
            Activo = cliente.Activo,
            FechaCreacion = cliente.FechaCreacion
        };
    }
}
