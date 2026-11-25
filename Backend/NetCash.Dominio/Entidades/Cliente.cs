using NetCash.Dominio.Comunes;

namespace NetCash.Dominio.Entidades;

public class Cliente : EntidadBase
{
    public string Nombres { get; set; }
        = string.Empty;

    public string Apellidos { get; set; }
        = string.Empty;

    public string Identificacion { get; set; }
        = string.Empty;

    public string Correo { get; set; }
        = string.Empty;

    public string Telefono { get; set; }
        = string.Empty;

    public string Direccion { get; set; }
        = string.Empty;

    public string ReferenciaDireccion { get; set; }
        = string.Empty;
}