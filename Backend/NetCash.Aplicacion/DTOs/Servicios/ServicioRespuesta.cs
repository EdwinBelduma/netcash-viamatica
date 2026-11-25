namespace NetCash.Aplicacion.DTOs.Servicios;

public class ServicioRespuesta
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public int VelocidadMbps { get; set; }

    public decimal Precio { get; set; }

    public bool Activo { get; set; }

    public List<EquipoRespuesta> Equipos { get; set; } = new();
}