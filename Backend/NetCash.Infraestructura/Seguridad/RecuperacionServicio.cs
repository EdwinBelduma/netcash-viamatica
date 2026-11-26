using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCash.Aplicacion.DTOs.Autenticacion;
using NetCash.Aplicacion.Interfaces;
using NetCash.Dominio.Entidades;
using NetCash.Dominio.Enumeraciones;
using NetCash.Infraestructura.Persistencia;

namespace NetCash.Infraestructura.Seguridad;

public class RecuperacionServicio : IRecuperacionServicio
{
    private readonly NetCashDbContext _contexto;
    private readonly ICifradoServicio _cifrado;
    private readonly ICorreoServicio _correo;
    private readonly IConfiguration _configuracion;
    private readonly ILogger<RecuperacionServicio> _logger;

    private readonly PasswordHasher<Usuario> _passwordHasher
        = new();

    public RecuperacionServicio(
        NetCashDbContext contexto,
        ICifradoServicio cifrado,
        ICorreoServicio correo,
        IConfiguration configuracion,
        ILogger<RecuperacionServicio> logger)
    {
        _contexto = contexto;
        _cifrado = cifrado;
        _correo = correo;
        _configuracion = configuracion;
        _logger = logger;
    }

    // ==========================================
    // SOLICITAR RECUPERACION DE CONTRASENA
    // ==========================================

    public async Task SolicitarRecuperacionAsync(
        RecuperarContrasenaSolicitud solicitud)
    {
        var correoSolicitado = solicitud.Correo.Trim();

        var usuarios = await _contexto.Usuarios
            .Where(x =>
                x.Activo &&
                x.Estado == EstadoUsuario.Activo)
            .ToListAsync();

        var usuario = usuarios.FirstOrDefault(x =>
            string.Equals(
                _cifrado.Descifrar(x.CorreoCifrado),
                correoSolicitado,
                StringComparison.OrdinalIgnoreCase));

        // No revelar si el correo existe en el sistema.

        if (usuario == null)
        {
            return;
        }

        var urlFrontend =
            _configuracion["Recuperacion:UrlFrontend"]
            ?? "http://localhost:4200";

        // Generar un token criptograficamente seguro.

        var token = Convert.ToHexString(
            RandomNumberGenerator.GetBytes(32));

        var tokenHash = GenerarHash(token);

        await using (var transaccion =
            await _contexto.Database.BeginTransactionAsync(
                IsolationLevel.Serializable))
        {
            var ahora = DateTime.UtcNow;

            // Limitar solicitudes consecutivas.

            var solicitudReciente =
                await _contexto.TokensRecuperacion
                    .AnyAsync(x =>
                        x.UsuarioId == usuario.Id &&
                        x.FechaCreacion >
                            ahora.AddMinutes(-1));

            if (solicitudReciente)
            {
                return;
            }

            // Invalidar los tokens anteriores.

            var tokensAnteriores =
                await _contexto.TokensRecuperacion
                    .Where(x =>
                        x.UsuarioId == usuario.Id &&
                        !x.Utilizado &&
                        x.Activo)
                    .ToListAsync();

            foreach (var anterior in tokensAnteriores)
            {
                anterior.Activo = false;
                anterior.FechaActualizacion = ahora;
            }

            // Almacenar solamente el hash del token.

            var recuperacion = new TokenRecuperacion
            {
                UsuarioId = usuario.Id,
                TokenHash = tokenHash,
                FechaCreacion = ahora,
                FechaExpiracion = ahora.AddMinutes(15),
                Utilizado = false,
                Activo = true
            };

            _contexto.TokensRecuperacion.Add(recuperacion);

            await _contexto.SaveChangesAsync();

            await transaccion.CommitAsync();
        }

        // Construir el enlace de recuperacion.

        var enlace =
            $"{urlFrontend.TrimEnd('/')}" +
            "/restablecer-contrasena" +
            $"?token={Uri.EscapeDataString(token)}";

        try
        {
            await _correo.EnviarRecuperacionAsync(
                correoSolicitado,
                usuario.Nombres,
                enlace);

            _logger.LogInformation(
                "Solicitud de recuperacion procesada. " +
                "UsuarioId: {UsuarioId}",
                usuario.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "No se pudo enviar el correo de recuperacion. " +
                "UsuarioId: {UsuarioId}. Error: {TipoError}",
                usuario.Id,
                ex.GetType().Name);
        }
    }

    // ==========================================
    // RESTABLECER CONTRASENA
    // ==========================================

    public async Task<bool> RestablecerContrasenaAsync(
        RestablecerContrasenaSolicitud solicitud)
    {
        if (string.IsNullOrWhiteSpace(solicitud.Token) ||
            solicitud.Token.Length != 64)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(
                solicitud.NuevaContrasena) ||
            solicitud.NuevaContrasena !=
                solicitud.ConfirmarContrasena)
        {
            return false;
        }

        if (solicitud.NuevaContrasena.Length < 8 ||
            solicitud.NuevaContrasena.Length > 30 ||
            !solicitud.NuevaContrasena.Any(char.IsUpper) ||
            !solicitud.NuevaContrasena.Any(char.IsDigit))
        {
            return false;
        }

        var tokenHash = GenerarHash(
            solicitud.Token.Trim());

        await using var transaccion =
            await _contexto.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var recuperacion =
            await _contexto.TokensRecuperacion
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x =>
                    x.TokenHash == tokenHash);

        if (recuperacion == null ||
            !recuperacion.EsValido())
        {
            return false;
        }

        var usuario = recuperacion.Usuario;

        if (!usuario.Activo ||
            usuario.Estado != EstadoUsuario.Activo)
        {
            return false;
        }

        // Generar el hash de la nueva contrasena.

        usuario.ContrasenaHash =
            _passwordHasher.HashPassword(
                usuario,
                solicitud.NuevaContrasena);

        usuario.FechaActualizacion = DateTime.UtcNow;

        usuario.SesionActiva = false;

        // Marcar el token como utilizado.

        recuperacion.MarcarComoUtilizado();

        // Invalidar cualquier otro token pendiente.

        var otrosTokens =
            await _contexto.TokensRecuperacion
                .Where(x =>
                    x.UsuarioId == usuario.Id &&
                    x.Id != recuperacion.Id &&
                    !x.Utilizado &&
                    x.Activo)
                .ToListAsync();

        foreach (var otroToken in otrosTokens)
        {
            otroToken.Activo = false;

            otroToken.FechaActualizacion =
                DateTime.UtcNow;
        }

        await _contexto.SaveChangesAsync();

        await transaccion.CommitAsync();

        _logger.LogInformation(
            "Contrasena restablecida correctamente. " +
            "UsuarioId: {UsuarioId}",
            usuario.Id);

        return true;
    }

    // ==========================================
    // GENERAR HASH SHA-256
    // ==========================================

    private static string GenerarHash(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);

        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}