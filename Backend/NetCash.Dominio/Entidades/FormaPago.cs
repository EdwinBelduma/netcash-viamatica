using NetCash.Dominio.Comunes;

namespace NetCash.Dominio.Entidades;

public class FormaPago : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;
}