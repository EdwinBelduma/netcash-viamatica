
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetCash.Aplicacion.DTOs.Usuarios;
using NetCash.Aplicacion.Interfaces;
using NetCash.Dominio.Entidades;
using NetCash.Dominio.Enumeraciones;
using NetCash.Infraestructura.Persistencia;

namespace NetCash.Infraestructura.Seguridad;

public class UsuarioServicio : IUsuarioServicio
{
    private readonly NetCashDbContext _context;
    private readonly ICifradoServicio _cifrado;
    private readonly PasswordHasher<Usuario> _passwordHasher;

    public UsuarioServicio(
        NetCashDbContext context,
        ICifradoServicio cifrado)
    {
        _context = context;
        _cifrado = cifrado;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    public async Task<List<UsuarioDetalleRespuesta>>
        ObtenerTodosAsync()
    {
        var usuarios = await _context.Usuarios
            .AsNoTracking()
            .Include(x => x.Rol)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return usuarios.Select(Mapear).ToList();
    }

    public async Task<UsuarioDetalleRespuesta?>
        ObtenerPorIdAsync(int id)
    {
        var usuario = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x => x.Id == id);

        return usuario is null ? null : Mapear(usuario);
    }

    public async Task<UsuarioDetalleRespuesta> CrearAsync(
        UsuarioSolicitud solicitud,
        int administradorId)
    {
        var nombreUsuario = solicitud.NombreUsuario
            .Trim()
            .ToLowerInvariant();

        var existe = await _context.Usuarios
            .IgnoreQueryFilters()
            .AnyAsync(x =>
                x.NombreUsuario.ToLower() == nombreUsuario);

        if (existe)
        {
            throw new InvalidOperationException(
                "El nombre de usuario ya existe.");
        }

        var rol = await _context.Roles
            .FirstOrDefaultAsync(x =>
                x.Id == solicitud.RolId && x.Activo);

        if (rol is null)
        {
            throw new InvalidOperationException(
                "El rol seleccionado no existe.");
        }

        await ValidarDuplicadosAsync(
            solicitud.Identificacion,
            solicitud.Correo);

        var usuario = new Usuario
        {
            NombreUsuario = nombreUsuario,
            Nombres = solicitud.Nombres.Trim(),
            Apellidos = solicitud.Apellidos.Trim(),

            IdentificacionCifrada = _cifrado.Cifrar(
                solicitud.Identificacion.Trim()),

            CorreoCifrado = _cifrado.Cifrar(
                solicitud.Correo.Trim().ToLowerInvariant()),

            RolId = rol.Id,
            Rol = rol,

            Estado = EstadoUsuario.PendienteAprobacion,

            UsuarioCreacionId = administradorId,
            FechaCreacion = DateTime.UtcNow,

            Activo = true,
            SesionActiva = false
        };

        usuario.ContrasenaHash =
            _passwordHasher.HashPassword(
                usuario,
                solicitud.Contrasena);

        _context.Usuarios.Add(usuario);

        await _context.SaveChangesAsync();

        return Mapear(usuario);
    }

    public async Task<UsuarioDetalleRespuesta?>
        ActualizarAsync(
            int id,
            UsuarioActualizarSolicitud solicitud,
            int administradorId)
    {
        var usuario = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (usuario is null)
        {
            return null;
        }

        if (id == administradorId &&
            usuario.RolId != solicitud.RolId)
        {
            throw new InvalidOperationException(
                "No puede modificar su propio rol.");
        }

        var rol = await _context.Roles
            .FirstOrDefaultAsync(x =>
                x.Id == solicitud.RolId && x.Activo);

        if (rol is null)
        {
            throw new InvalidOperationException(
                "El rol seleccionado no existe.");
        }

        await ValidarDuplicadosAsync(
            solicitud.Identificacion,
            solicitud.Correo,
            id);

        usuario.Nombres = solicitud.Nombres.Trim();
        usuario.Apellidos = solicitud.Apellidos.Trim();

        usuario.IdentificacionCifrada =
            _cifrado.Cifrar(
                solicitud.Identificacion.Trim());

        usuario.CorreoCifrado =
            _cifrado.Cifrar(
                solicitud.Correo.Trim().ToLowerInvariant());

        usuario.RolId = rol.Id;
        usuario.Rol = rol;

        usuario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Mapear(usuario);
    }

    public async Task<bool> AprobarAsync(
        int id,
        int administradorId)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(x => x.Id == id);

        if (usuario is null)
        {
            return false;
        }

        if (usuario.Estado !=
            EstadoUsuario.PendienteAprobacion)
        {
            throw new InvalidOperationException(
                "El usuario no esta pendiente de aprobacion.");
        }

        usuario.Estado = EstadoUsuario.Activo;
        usuario.Activo = true;

        usuario.UsuarioAprobacionId = administradorId;
        usuario.FechaAprobacion = DateTime.UtcNow;
        usuario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> EliminarAsync(
        int id,
        int administradorId)
    {
        if (id == administradorId)
        {
            throw new InvalidOperationException(
                "No puede eliminar su propia cuenta.");
        }

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(x => x.Id == id);

        if (usuario is null)
        {
            return false;
        }

        if (usuario.RolId == 1)
        {
            var administradores = await _context.Usuarios
                .CountAsync(x =>
                    x.RolId == 1 &&
                    x.Activo &&
                    x.Estado == EstadoUsuario.Activo);

            if (administradores <= 1)
            {
                throw new InvalidOperationException(
                    "Debe existir un administrador activo.");
            }
        }

        usuario.Estado = EstadoUsuario.Inactivo;
        usuario.SesionActiva = false;

        usuario.Eliminar();

        await _context.SaveChangesAsync();

        return true;
    }

    private async Task ValidarDuplicadosAsync(
        string identificacion,
        string correo,
        int? excluirId = null)
    {
        var usuarios = await _context.Usuarios
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x =>
                !excluirId.HasValue ||
                x.Id != excluirId.Value)
            .Select(x => new
            {
                x.IdentificacionCifrada,
                x.CorreoCifrado
            })
            .ToListAsync();

        foreach (var usuario in usuarios)
        {
            var identificacionActual = _cifrado.Descifrar(
                usuario.IdentificacionCifrada);

            var correoActual = _cifrado.Descifrar(
                usuario.CorreoCifrado);

            if (identificacionActual ==
                identificacion.Trim())
            {
                throw new InvalidOperationException(
                    "La identificacion ya esta registrada.");
            }

            if (string.Equals(
                correoActual,
                correo.Trim(),
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "El correo ya esta registrado.");
            }
        }
    }

    private UsuarioDetalleRespuesta Mapear(Usuario usuario)
    {
        return new UsuarioDetalleRespuesta
        {
            Id = usuario.Id,
            NombreUsuario = usuario.NombreUsuario,
            Nombres = usuario.Nombres,
            Apellidos = usuario.Apellidos,

            Identificacion = _cifrado.Descifrar(
                usuario.IdentificacionCifrada),

            Correo = _cifrado.Descifrar(
                usuario.CorreoCifrado),

            RolId = usuario.RolId,
            Rol = usuario.Rol.Nombre,
            Estado = usuario.Estado.ToString(),
            Activo = usuario.Activo,
            FechaCreacion = usuario.FechaCreacion,
            FechaAprobacion = usuario.FechaAprobacion
        };
    }
}
