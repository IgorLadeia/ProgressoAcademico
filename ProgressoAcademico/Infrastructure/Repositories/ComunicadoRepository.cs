using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Context;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Infrastructure.Repositories;

public class ComunicadoRepository : IComunicadoRepository
{
    private readonly ProgressoAcademicoDbContext _context;

    public ComunicadoRepository(ProgressoAcademicoDbContext context)
    {
        _context = context;
    }

    public Task<List<Comunicado>> ListarPublicadosAsync()
    {
        return _context.Comunicados
            .AsNoTracking()
            .Where(c => c.Publicado && c.DataPublicacao <= DateTime.UtcNow)
            .OrderByDescending(c => c.DataPublicacao)
            .ToListAsync();
    }

    public Task<List<Comunicado>> ListarTodosAsync()
    {
        return _context.Comunicados
            .AsNoTracking()
            .OrderByDescending(c => c.DataPublicacao)
            .ThenByDescending(c => c.ComunicadoId)
            .ToListAsync();
    }

    public Task<Comunicado?> ObterPorIdAsync(int comunicadoId)
    {
        return _context.Comunicados.FirstOrDefaultAsync(c => c.ComunicadoId == comunicadoId);
    }

    public async Task AdicionarAsync(Comunicado comunicado)
    {
        await _context.Comunicados.AddAsync(comunicado);
    }

    public void Remover(Comunicado comunicado)
    {
        _context.Comunicados.Remove(comunicado);
    }

    public Task SalvarAlteracoesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
