using NetCash.Dominio.Comunes;

namespace NetCash.Dominio.Entidades;

public class Servicio : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public int VelocidadMbps { get; set; }

    public decimal Precio { get; set; }

    public ICollection<Equipo> Equipos { get; set; }
        = new List<Equipo>();
}