using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Context;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Infrastructure.Repositories;

public class SolicitacaoProgressaoRepository : ISolicitacaoProgressaoRepository
{
    private readonly ProgressoAcademicoDbContext _context;

    public SolicitacaoProgressaoRepository(ProgressoAcademicoDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SolicitacaoProgressao>> ListarPorUsuarioAsync(int usuarioId)
    {
        return await _context.SolicitacoesProgressao
            .AsNoTracking()
            .Include(s => s.StatusSolicitacao)
            .Include(s => s.TipoProgresso)
            .Include(s => s.NivelOrigem)
            .Include(s => s.NivelDestino)
            .Include(s => s.Atividades)
            .Include(s => s.Documentos)
            .Include(s => s.Revisores)
            .Where(s => s.UsuarioId == usuarioId)
            .OrderByDescending(s => s.DataUltimaMovimentacao ?? s.DataCriacao)
            .ThenByDescending(s => s.DataCriacao)
            .ToListAsync();
    }

    public Task<SolicitacaoProgressao?> ObterDetalhePorIdAsync(int solicitacaoProgressaoId)
    {
        return _context.SolicitacoesProgressao
            .AsNoTracking()
            .Include(s => s.StatusSolicitacao)
            .Include(s => s.TipoProgresso)
            .Include(s => s.NivelOrigem)
            .Include(s => s.NivelDestino)
            .Include(s => s.Usuario)
                .ThenInclude(u => u.VinculosInstitucionais)
            .Include(s => s.Atividades)
                .ThenInclude(a => a.TipoAtividade)
            .Include(s => s.Documentos)
                .ThenInclude(d => d.TipoDocumento)
            .FirstOrDefaultAsync(s => s.SolicitacaoProgressaoId == solicitacaoProgressaoId);
    }

    public Task<SolicitacaoProgressao?> ObterDetalhePorIdEUsuarioAsync(int solicitacaoProgressaoId, int usuarioId)
    {
        return _context.SolicitacoesProgressao
            .AsNoTracking()
            .Include(s => s.StatusSolicitacao)
            .Include(s => s.TipoProgresso)
            .Include(s => s.NivelOrigem)
            .Include(s => s.NivelDestino)
            .Include(s => s.Usuario)
                .ThenInclude(u => u.VinculosInstitucionais)
            .Include(s => s.Atividades)
                .ThenInclude(a => a.TipoAtividade)
            .Include(s => s.Documentos)
                .ThenInclude(d => d.TipoDocumento)
            .FirstOrDefaultAsync(s => s.SolicitacaoProgressaoId == solicitacaoProgressaoId && s.UsuarioId == usuarioId);
    }

    public Task<SolicitacaoProgressao?> ObterParaEdicaoPorIdEUsuarioAsync(int solicitacaoProgressaoId, int usuarioId)
    {
        return _context.SolicitacoesProgressao
            .Include(s => s.StatusSolicitacao)
            .FirstOrDefaultAsync(s => s.SolicitacaoProgressaoId == solicitacaoProgressaoId && s.UsuarioId == usuarioId);
    }

    public Task<SolicitacaoProgressao?> ObterParaSubmissaoPorIdEUsuarioAsync(int solicitacaoProgressaoId, int usuarioId)
    {
        return _context.SolicitacoesProgressao
            .Include(s => s.StatusSolicitacao)
            .Include(s => s.Atividades)
            .Include(s => s.Documentos)
            .Include(s => s.Revisores)
            .FirstOrDefaultAsync(s => s.SolicitacaoProgressaoId == solicitacaoProgressaoId && s.UsuarioId == usuarioId);
    }

    public Task<bool> ExisteSolicitacaoAbertaAsync(int usuarioId)
    {
        return _context.SolicitacoesProgressao
            .AnyAsync(s => s.UsuarioId == usuarioId
                && (s.StatusSolicitacao.Nome == StatusSolicitacaoNomes.AguardandoAtribuicao
                    || s.StatusSolicitacao.Nome == StatusSolicitacaoNomes.EmProcessamento
                    || s.StatusSolicitacao.Nome == StatusSolicitacaoNomes.EmRevisao
                    || s.StatusSolicitacao.Nome == StatusSolicitacaoNomes.AguardandoDecisaoFinal
                    || s.StatusSolicitacao.Nome == StatusSolicitacaoNomes.AjustesSolicitados));
    }

    public Task<StatusSolicitacao?> ObterStatusPorNomeAsync(string nome)
    {
        return _context.StatusSolicitacoes
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Nome == nome);
    }

    public Task<bool> TipoProgressoExisteAsync(int tipoProgressoId)
    {
        return _context.TiposProgresso.AnyAsync(t => t.TipoProgressoId == tipoProgressoId);
    }

    public Task<bool> NivelExisteAsync(int nivelId)
    {
        return _context.Niveis.AnyAsync(n => n.NivelId == nivelId && n.Ativo);
    }

    public async Task AdicionarAsync(SolicitacaoProgressao solicitacao)
    {
        await _context.SolicitacoesProgressao.AddAsync(solicitacao);
    }

    public Task SalvarAlteracoesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
