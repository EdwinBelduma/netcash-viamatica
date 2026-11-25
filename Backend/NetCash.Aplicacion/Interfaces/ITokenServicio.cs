using NetCash.Dominio.Entidades;

namespace NetCash.Aplicacion.Interfaces;

public interface ITokenServicio
{
    (string Token, DateTime FechaExpiracion)
        GenerarToken(Usuario usuario);
}