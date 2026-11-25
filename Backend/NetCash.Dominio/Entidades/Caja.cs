using NetCash.Dominio.Comunes;

namespace NetCash.Dominio.Entidades;

public class Caja : EntidadBase
{
    public string Descripcion { get; set; } = string.Empty;

    public int? UsuarioEnSesionId { get; set; }

    public DateTime? FechaApertura { get; set; }

    public ICollection<UsuarioCaja> UsuariosCaja { get; set; }
        = new List<UsuarioCaja>();
}