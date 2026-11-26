using System.IO;
using System.Threading.Tasks;
using NetCash.Aplicacion.DTOs.Importaciones;

namespace NetCash.Aplicacion.Interfaces;

public interface IImportacionServicio
{
    Task<ImportacionRespuesta> ImportarUsuariosAsync(
        Stream archivo,
        int usuarioResponsableId
    );

    Task<ImportacionRespuesta> ImportarClientesAsync(
        Stream archivo,
        int usuarioResponsableId
    );
}