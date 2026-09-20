using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Context;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Infrastructure.Repositories;

public class ConteudoHomeRepository : IConteudoHomeRepository
{
    private const int ConteudoPrincipalId = 1;
    private readonly ProgressoAcademicoDbContext _context;

    public ConteudoHomeRepository(ProgressoAcademicoDbContext context)
    {
        _context = context;
    }

    public Task<ConteudoHome?> ObterAsync()
    {
        return _context.ConteudosHome
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ConteudoHomeId == ConteudoPrincipalId);
    }

    public Task<ConteudoHome?> ObterParaAtualizacaoAsync()
    {
        return _context.ConteudosHome
            .FirstOrDefaultAsync(c => c.ConteudoHomeId == ConteudoPrincipalId);
    }

    public Task SalvarAlteracoesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
