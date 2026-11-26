using NetCash.Dominio.Comunes;

namespace NetCash.Dominio.Entidades;

public class TokenRecuperacion : EntidadBase
{
    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;

    public DateTime FechaExpiracion { get; set; }

    public bool Utilizado { get; set; } = false;

    public DateTime? FechaUtilizacion { get; set; }

    public bool EsValido()
    {
        return Activo
            && !Utilizado
            && FechaExpiracion > DateTime.UtcNow;
    }

    public void MarcarComoUtilizado()
    {
        Utilizado = true;

        FechaUtilizacion = DateTime.UtcNow;

        FechaActualizacion = DateTime.UtcNow;
    }
}