namespace NetCash.Dominio.Comunes;

public abstract class EntidadBase
{
    public int Id { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; }
        = DateTime.UtcNow;

    public DateTime? FechaActualizacion { get; set; }

    public DateTime? FechaEliminacion { get; set; }

    public void Eliminar()
    {
        Activo = false;
        FechaEliminacion = DateTime.UtcNow;
        FechaActualizacion = DateTime.UtcNow;
    }
}