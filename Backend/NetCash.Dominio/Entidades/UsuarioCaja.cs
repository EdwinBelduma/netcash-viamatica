using NetCash.Dominio.Comunes;

namespace NetCash.Dominio.Entidades;

public class UsuarioCaja : EntidadBase
{
    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public int CajaId { get; set; }

    public Caja Caja { get; set; } = null!;

    public int GestorId { get; set; }

    public Usuario Gestor { get; set; } = null!;

    public DateTime FechaAsignacion { get; set; }
        = DateTime.UtcNow;
}