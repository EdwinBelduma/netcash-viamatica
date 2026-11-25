
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetCash.Aplicacion.Interfaces;
using NetCash.Dominio.Entidades;
using NetCash.Dominio.Enumeraciones;
using NetCash.Infraestructura.Persistencia;

namespace NetCash.Infraestructura.Seguridad;

public class InicializadorUsuarios
{
    private readonly NetCashDbContext _context;
    private readonly ICifradoServicio _cifradoServicio;

    public InicializadorUsuarios(
        NetCashDbContext context,
        ICifradoServicio cifradoServicio)
    {
        _context = context;
        _cifradoServicio = cifradoServicio;
    }

    public async Task<bool> InicializarAsync(
        string contrasena)
    {
        var existeAdministrador = await _context.Usuarios
            .IgnoreQueryFilters()
            .AnyAsync(x => x.RolId == 1);

        if (existeAdministrador)
        {
            return false;
        }

        if (contrasena.Length < 8 ||
            contrasena.Length > 30 ||
            !contrasena.Any(char.IsUpper) ||
            !contrasena.Any(char.IsDigit))
        {
            throw new ArgumentException(
                "La contraseña inicial no cumple los requisitos.");
        }

        var administrador = new Usuario
        {
            NombreUsuario = "adminnet01",
            Nombres = "Administrador",
            Apellidos = "NetCash",

            IdentificacionCifrada =
                _cifradoServicio.Cifrar("0000000000"),

            CorreoCifrado =
                _cifradoServicio.Cifrar("admin@netcash.example"),

            RolId = 1,
            Estado = EstadoUsuario.Activo,
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            FechaAprobacion = DateTime.UtcNow,
            SesionActiva = false
        };

        var passwordHasher = new PasswordHasher<Usuario>();

        administrador.ContrasenaHash =
            passwordHasher.HashPassword(
                administrador,
                contrasena);

        _context.Usuarios.Add(administrador);

        await _context.SaveChangesAsync();

        return true;
    }
}
