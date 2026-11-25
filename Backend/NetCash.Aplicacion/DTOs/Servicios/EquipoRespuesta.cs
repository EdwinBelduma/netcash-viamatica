namespace NetCash.Aplicacion.DTOs.Servicios;

public class EquipoRespuesta
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public int ServicioId { get; set; }

    public bool Activo { get; set; }
}