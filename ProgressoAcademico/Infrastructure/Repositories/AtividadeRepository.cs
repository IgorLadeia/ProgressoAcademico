using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Context;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Infrastructure.Repositories;

public class AtividadeRepository : IAtividadeRepository
{
    private readonly ProgressoAcademicoDbContext _context;

    public AtividadeRepository(ProgressoAcademicoDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Atividade>> ListarPorSolicitacaoAsync(int solicitacaoProgressaoId, int usuarioId)
    {
        return await _context.Atividades
            .AsNoTracking()
            .Include(a => a.TipoAtividade)
            .Include(a => a.SubTipoAtividade)
            .Include(a => a.AvaliacoesRevisores)
                .ThenInclude(ar => ar.SolicitacaoRevisor)
                    .ThenInclude(sr => sr.RevisorUsuario)
            .Where(a => a.SolicitacaoProgressaoId == solicitacaoProgressaoId
                && a.SolicitacaoProgressao!.UsuarioId == usuarioId)
            .OrderByDescending(a => a.DataInicio)
            .ToListAsync();
    }

    public Task<List<SubTipoAtividade>> ListarSubtiposAsync()
    {
        return _context.SubtiposAtividade
            .AsNoTracking()
            .Include(s => s.TipoAtividade)
            .OrderBy(s => s.TipoAtividade.Nome)
            .ThenBy(s => s.Nome)
            .ToListAsync();
    }

    public Task<SubTipoAtividade?> ObterSubtipoAsync(int subTipoAtividadeId)
    {
        return _context.SubtiposAtividade
            .AsNoTracking()
            .Include(s => s.TipoAtividade)
            .FirstOrDefaultAsync(s => s.SubtipoAtividadeId == subTipoAtividadeId);
    }

    public Task<Atividade?> ObterPorIdEUsuarioAsync(int atividadeId, int usuarioId)
    {
        return _context.Atividades
            .Include(a => a.SolicitacaoProgressao)
                .ThenInclude(s => s!.StatusSolicitacao)
            .FirstOrDefaultAsync(a => a.AtividadeId == atividadeId
                && a.SolicitacaoProgressao!.UsuarioId == usuarioId);
    }

    public async Task AdicionarAsync(Atividade atividade)
    {
        await _context.Atividades.AddAsync(atividade);
    }

    public void Remover(Atividade atividade)
    {
        _context.Atividades.Remove(atividade);
    }

    public Task SalvarAlteracoesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
