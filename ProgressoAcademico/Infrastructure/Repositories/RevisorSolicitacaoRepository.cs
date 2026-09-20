using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Context;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Infrastructure.Repositories;

public class RevisorSolicitacaoRepository : IRevisorSolicitacaoRepository
{
    private readonly ProgressoAcademicoDbContext _context;

    public RevisorSolicitacaoRepository(ProgressoAcademicoDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SolicitacaoProgressao>> ListarAtribuidasAsync(int revisorUsuarioId)
    {
        return await QueryBase()
            .AsNoTracking()
            .Where(s => s.Revisores.Any(sr => sr.RevisorUsuarioId == revisorUsuarioId))
            .OrderByDescending(s => s.Revisores
                .Where(sr => sr.RevisorUsuarioId == revisorUsuarioId)
                .Select(sr => sr.DataAtribuicao)
                .FirstOrDefault())
            .ToListAsync();
    }

    public Task<SolicitacaoProgressao?> ObterDetalheAtribuidaAsync(int solicitacaoProgressaoId, int revisorUsuarioId)
    {
        return QueryBase()
            .FirstOrDefaultAsync(s => s.SolicitacaoProgressaoId == solicitacaoProgressaoId
                && s.Revisores.Any(sr => sr.RevisorUsuarioId == revisorUsuarioId));
    }

    public Task<SolicitacaoRevisor?> ObterAtribuicaoAsync(int solicitacaoProgressaoId, int revisorUsuarioId)
    {
        return _context.SolicitacoesRevisores
            .Include(sr => sr.SolicitacaoProgressao)
                .ThenInclude(s => s.StatusSolicitacao)
            .Include(sr => sr.SolicitacaoProgressao)
                .ThenInclude(s => s.Revisores)
            .FirstOrDefaultAsync(sr => sr.SolicitacaoProgressaoId == solicitacaoProgressaoId
                && sr.RevisorUsuarioId == revisorUsuarioId);
    }

    public Task<StatusSolicitacao?> ObterStatusPorNomeAsync(string nome)
    {
        return _context.StatusSolicitacoes.FirstOrDefaultAsync(s => s.Nome == nome);
    }

    public Task<Atividade?> ObterAtividadeAsync(int atividadeId, int solicitacaoProgressaoId, int revisorUsuarioId)
    {
        return _context.Atividades
            .Include(a => a.SolicitacaoProgressao)
            .FirstOrDefaultAsync(a => a.AtividadeId == atividadeId
                && a.SolicitacaoProgressaoId == solicitacaoProgressaoId
                && a.SolicitacaoProgressao!.Revisores.Any(sr => sr.RevisorUsuarioId == revisorUsuarioId));
    }

    public Task<AtividadeAvaliacaoRevisor?> ObterAvaliacaoAtividadeAsync(int atividadeId, int solicitacaoRevisorId)
    {
        return _context.AtividadesAvaliacoesRevisores.FirstOrDefaultAsync(a =>
            a.AtividadeId == atividadeId && a.SolicitacaoRevisorId == solicitacaoRevisorId);
    }

    public async Task AdicionarAvaliacaoAtividadeAsync(AtividadeAvaliacaoRevisor avaliacao)
    {
        await _context.AtividadesAvaliacoesRevisores.AddAsync(avaliacao);
    }

    public Task<Documento?> ObterDocumentoAsync(int documentoId, int revisorUsuarioId)
    {
        return _context.Documentos
            .AsNoTracking()
            .Include(d => d.SolicitacaoProgressao)
            .FirstOrDefaultAsync(d => d.DocumentoId == documentoId
                && d.SolicitacaoProgressao!.Revisores.Any(sr => sr.RevisorUsuarioId == revisorUsuarioId));
    }

    public Task SalvarAlteracoesAsync()
    {
        return _context.SaveChangesAsync();
    }

    private IQueryable<SolicitacaoProgressao> QueryBase()
    {
        return _context.SolicitacoesProgressao
            .Include(s => s.Usuario)
            .Include(s => s.StatusSolicitacao)
            .Include(s => s.TipoProgresso)
            .Include(s => s.NivelOrigem)
            .Include(s => s.NivelDestino)
            .Include(s => s.RevisorUsuario)
            .Include(s => s.AtribuidoPorUsuario)
            .Include(s => s.RevisadoPorUsuario)
            .Include(s => s.Revisores)
                .ThenInclude(sr => sr.RevisorUsuario)
            .Include(s => s.Revisores)
                .ThenInclude(sr => sr.AtribuidoPorUsuario)
            .Include(s => s.Atividades)
                .ThenInclude(a => a.TipoAtividade)
            .Include(s => s.Atividades)
                .ThenInclude(a => a.SubTipoAtividade)
            .Include(s => s.Atividades)
                .ThenInclude(a => a.AvaliacoesRevisores)
                    .ThenInclude(ar => ar.SolicitacaoRevisor)
                        .ThenInclude(sr => sr.RevisorUsuario)
            .Include(s => s.Documentos)
                .ThenInclude(d => d.TipoDocumento);
    }
}
