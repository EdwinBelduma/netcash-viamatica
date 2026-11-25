
using Microsoft.EntityFrameworkCore;
using NetCash.Aplicacion.Interfaces;
using NetCash.Dominio.Entidades;
using NetCash.Infraestructura.Persistencia;

namespace NetCash.Infraestructura.Repositorios;

public class ClienteRepositorio : IClienteRepositorio
{
    private readonly NetCashDbContext _context;

    public ClienteRepositorio(NetCashDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cliente>> ObtenerTodosAsync()
    {
        return await _context.Clientes
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(x => x.Id == id && x.Activo);
    }

    public async Task<bool> ExisteIdentificacionAsync(
        string identificacion,
        int? excluirId = null)
    {
        return await _context.Clientes
            .IgnoreQueryFilters()
            .AnyAsync(x =>
                x.Identificacion == identificacion &&
                (!excluirId.HasValue ||
                 x.Id != excluirId.Value));
    }

    public async Task AgregarAsync(Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
