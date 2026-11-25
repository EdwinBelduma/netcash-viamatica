
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetCash.Aplicacion.DTOs.Autenticacion;
using NetCash.Aplicacion.Interfaces;
using NetCash.Dominio.Entidades;
using NetCash.Dominio.Enumeraciones;
using NetCash.Infraestructura.Persistencia;

namespace NetCash.Infraestructura.Seguridad;

public class AutenticacionServicio : IAutenticacionServicio
{
    private readonly NetCashDbContext _context;
    private readonly ITokenServicio _tokenServicio;
    private readonly PasswordHasher<Usuario> _passwordHasher;

    public AutenticacionServicio(
        NetCashDbContext context,
        ITokenServicio tokenServicio)
    {
        _context = context;
        _tokenServicio = tokenServicio;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    public async Task<LoginRespuesta?> IniciarSesionAsync(
        LoginSolicitud solicitud)
    {
        var nombreUsuario = solicitud.NombreUsuario
            .Trim()
            .ToLowerInvariant();

        var usuario = await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x =>
                x.NombreUsuario.ToLower() == nombreUsuario);

        if (usuario is null ||
            !usuario.Activo ||
            !usuario.Rol.Activo ||
            usuario.Estado != EstadoUsuario.Activo)
        {
            return null;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.ContrasenaHash,
            solicitud.Contrasena);

        if (resultado == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (resultado ==
            PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.ContrasenaHash =
                _passwordHasher.HashPassword(
                    usuario,
                    solicitud.Contrasena);
        }

        var token = _tokenServicio.GenerarToken(usuario);

        usuario.UltimoInicioSesion = DateTime.UtcNow;
        usuario.FechaActualizacion = DateTime.UtcNow;
        usuario.SesionActiva = true;

        await _context.SaveChangesAsync();

        return new LoginRespuesta
        {
            Token = token.Token,
            FechaExpiracion = token.FechaExpiracion,

            Usuario = new UsuarioRespuesta
            {
                Id = usuario.Id,
                NombreUsuario = usuario.NombreUsuario,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Rol = usuario.Rol.Tipo.ToString()
            }
        };
    }
}
