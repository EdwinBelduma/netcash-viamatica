namespace NetCash.Aplicacion.DTOs.Menus;

public class MenuRespuesta
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Ruta { get; set; } = string.Empty;

    public string Icono { get; set; } = string.Empty;

    public int Orden { get; set; }

    public int? MenuPadreId { get; set; }
}