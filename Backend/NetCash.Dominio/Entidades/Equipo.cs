using NetCash.Dominio.Comunes;

namespace NetCash.Dominio.Entidades;

public class Equipo : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public int ServicioId { get; set; }

    public Servicio Servicio { get; set; } = null!;
}