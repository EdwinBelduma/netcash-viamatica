
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using NetCash.Aplicacion.Interfaces;
using NetCash.Dominio.Entidades;

namespace NetCash.Infraestructura.Seguridad;

public class TokenServicio : ITokenServicio
{
    private readonly string _emisor;
    private readonly string _audiencia;

    private readonly SymmetricSecurityKey _claveFirma;
    private readonly SymmetricSecurityKey _claveCifrado;

    public TokenServicio(
        string claveFirma,
        string claveCifrado,
        string emisor,
        string audiencia)
    {
        _claveFirma = new SymmetricSecurityKey(
            Convert.FromBase64String(claveFirma));

        _claveCifrado = new SymmetricSecurityKey(
            Convert.FromBase64String(claveCifrado));

        _emisor = emisor;
        _audiencia = audiencia;
    }

    public (string Token, DateTime FechaExpiracion)
        GenerarToken(Usuario usuario)
    {
        var fechaExpiracion = DateTime.UtcNow.AddHours(2);

        var claims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                usuario.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                usuario.NombreUsuario),

            new Claim(
                ClaimTypes.Role,
                usuario.Rol.Tipo.ToString()),

            new Claim(
                "nombres",
                usuario.Nombres),

            new Claim(
                "apellidos",
                usuario.Apellidos),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var credencialesFirma = new SigningCredentials(
            _claveFirma,
            SecurityAlgorithms.HmacSha256);

        var credencialesCifrado = new EncryptingCredentials(
            _claveCifrado,
            SecurityAlgorithms.Aes256KW,
            SecurityAlgorithms.Aes128CbcHmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _emisor,
            Audience = _audiencia,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Expires = fechaExpiracion,
            SigningCredentials = credencialesFirma,
            EncryptingCredentials = credencialesCifrado
        };

        var manejador = new JwtSecurityTokenHandler();

        var token = manejador.CreateToken(descriptor);

        return (
            manejador.WriteToken(token),
            fechaExpiracion);
    }
}
