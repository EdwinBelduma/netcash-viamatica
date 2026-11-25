using NetCash.Dominio.Comunes;

namespace NetCash.Dominio.Entidades;

public class Menu : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;

    public string Ruta { get; set; } = string.Empty;

    public string Icono { get; set; } = string.Empty;

    public int Orden { get; set; }

    public int? MenuPadreId { get; set; }

    public Menu? MenuPadre { get; set; }

    public ICollection<RolMenu> RolesMenu { get; set; }
        = new List<RolMenu>();
}