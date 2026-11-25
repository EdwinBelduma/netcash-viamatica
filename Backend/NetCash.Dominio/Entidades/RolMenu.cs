using NetCash.Dominio.Comunes;

namespace NetCash.Dominio.Entidades;

public class RolMenu : EntidadBase
{
    public int RolId { get; set; }

    public Rol Rol { get; set; } = null!;

    public int MenuId { get; set; }

    public Menu Menu { get; set; } = null!;
}