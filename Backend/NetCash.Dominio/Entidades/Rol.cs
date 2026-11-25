using NetCash.Dominio.Comunes;
using NetCash.Dominio.Enumeraciones;

namespace NetCash.Dominio.Entidades;

public class Rol : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public TipoRol Tipo { get; set; }
}