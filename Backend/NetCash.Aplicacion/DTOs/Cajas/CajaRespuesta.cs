namespace NetCash.Aplicacion.DTOs.Cajas;

public class CajaRespuesta
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public bool Activo { get; set; }

    public bool Ocupada { get; set; }

    public int? UsuarioEnSesionId { get; set; }

    public DateTime? FechaApertura { get; set; }

    public int CantidadCajeros { get; set; }

    public List<int> CajerosAsignados { get; set; } = new();
}