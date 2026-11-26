namespace NetCash.Aplicacion.DTOs.Importaciones;

public class ImportacionRespuesta
{
    public int TotalRegistros { get; set; }

    public int RegistrosImportados { get; set; }

    public int RegistrosRechazados { get; set; }

    public List<string> Errores { get; set; } = new();
}