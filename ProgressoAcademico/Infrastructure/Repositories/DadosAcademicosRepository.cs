using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Context;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Infrastructure.Repositories;

public class DadosAcademicosRepository : IDadosAcademicosRepository
{
    private readonly ProgressoAcademicoDbContext _context;

    public DadosAcademicosRepository(ProgressoAcademicoDbContext context)
    {
        _context = context;
    }

    public Task<List<Instituicao>> ListarInstituicoesAsync()
    {
        return _context.Instituicoes
            .AsNoTracking()
            .OrderBy(i => i.Sigla)
            .ToListAsync();
    }

    public Task<List<TipoVinculo>> ListarTiposVinculoAsync()
    {
        return _context.TiposVinculo
            .AsNoTracking()
            .OrderBy(t => t.Nome)
            .ToListAsync();
    }

    public Task<List<Nivel>> ListarNiveisAsync()
    {
        return _context.Niveis
            .AsNoTracking()
            .Where(n => n.Ativo)
            .OrderBy(n => n.Ordem)
            .ToListAsync();
    }

    public Task<List<TipoProgresso>> ListarTiposProgressoAsync()
    {
        return _context.TiposProgresso
            .AsNoTracking()
            .OrderBy(t => t.Nome)
            .ToListAsync();
    }
}
