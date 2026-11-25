using NetCash.Dominio.Entidades;

namespace NetCash.Aplicacion.Interfaces;

public interface IClienteRepositorio
{
    Task<List<Cliente>> ObtenerTodosAsync();

    Task<Cliente?> ObtenerPorIdAsync(int id);

    Task<bool> ExisteIdentificacionAsync(
        string identificacion,
        int? excluirId = null);

    Task AgregarAsync(Cliente cliente);

    Task GuardarCambiosAsync();
}