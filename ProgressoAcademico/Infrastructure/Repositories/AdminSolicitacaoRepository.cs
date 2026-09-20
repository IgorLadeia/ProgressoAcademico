using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Application.DTOs.Admin;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Context;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Infrastructure.Repositories;

public class AdminSolicitacaoRepository : IAdminSolicitacaoRepository
{
    private readonly ProgressoAcademicoDbContext _context;

    public AdminSolicitacaoRepository(ProgressoAcademicoDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SolicitacaoProgressao>> ListarAsync(AdminSolicitacaoFiltroDto filtro)
    {
        var query = QueryBase()
            .AsNoTracking()
            .Where(s => s.StatusSolicitacao.Nome != StatusSolicitacaoNomes.EmProcessamento);

        if (!string.IsNullOrWhiteSpace(filtro.Status))
            query = query.Where(s => s.StatusSolicitacao.Nome == filtro.Status);

        if (filtro.SomentePendentes)
            query = query.Where(s => s.StatusSolicitacao.Nome == StatusSolicitacaoNomes.AguardandoAtribuicao
                || s.StatusSolicitacao.Nome == StatusSolicitacaoNomes.EmRevisao
                || s.StatusSolicitacao.Nome == StatusSolicitacaoNomes.AguardandoDecisaoFinal
                || s.StatusSolicitacao.Nome == StatusSolicitacaoNomes.AjustesSolicitados);

        if (!string.IsNullOrWhiteSpace(filtro.Professor))
            query = query.Where(s => s.Usuario.Nome.Contains(filtro.Professor) || s.Usuario.Email.Contains(filtro.Professor));

        if (filtro.TipoProgressoId.HasValue)
            query = query.Where(s => s.TipoProgressoId == filtro.TipoProgressoId.Value);

        if (filtro.DataInicio.HasValue)
            query = query.Where(s => s.DataCriacao >= filtro.DataInicio.Value);

        if (filtro.DataFim.HasValue)
            query = query.Where(s => s.DataCriacao <= filtro.DataFim.Value.Date.AddDays(1).AddTicks(-1));

        var solicitacoes = await query
            .OrderByDescending(s => s.DataCriacao)
            .ToListAsync();

        if (filtro.PercentualMinimo.HasValue)
            solicitacoes = solicitacoes
                .Where(s => CalcularPercentualPreenchimento(s) >= filtro.PercentualMinimo.Value)
                .ToList();

        return solicitacoes;
    }

    public Task<SolicitacaoProgressao?> ObterDetalheAsync(int solicitacaoProgressaoId)
    {
        return QueryBase()
            .Include(s => s.RevisadoPorUsuario)
            .Include(s => s.RevisorUsuario)
            .Include(s => s.AtribuidoPorUsuario)
            .Include(s => s.Revisores)
                .ThenInclude(sr => sr.RevisorUsuario)
            .Include(s => s.Revisores)
                .ThenInclude(sr => sr.AtribuidoPorUsuario)
            .FirstOrDefaultAsync(s => s.SolicitacaoProgressaoId == solicitacaoProgressaoId);
    }

    public Task<int> ContarProfessoresAsync()
    {
        return _context.Usuarios.CountAsync(u => u.PerfilAcesso == PerfisAcesso.Professor && u.Ativo);
    }

    public Task<int> ContarRevisoresAsync()
    {
        return _context.Usuarios.CountAsync(u => u.PodeRevisar && u.Ativo);
    }

    public Task<int> ContarDocumentosAsync()
    {
        return _context.Documentos.CountAsync();
    }

    public Task<int> ContarAtividadesAsync()
    {
        return _context.Atividades.CountAsync();
    }

    public Task<List<Usuario>> ListarRevisoresAsync()
    {
        return _context.Usuarios
            .AsNoTracking()
            .Where(u => u.PodeRevisar && u.Ativo)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public Task<Usuario?> ObterUsuarioAsync(int usuarioId)
    {
        return _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);
    }

    public Task<List<Usuario>> ObterUsuariosAsync(IReadOnlyCollection<int> usuarioIds)
    {
        return _context.Usuarios.Where(u => usuarioIds.Contains(u.UsuarioId)).ToListAsync();
    }

    public Task<StatusSolicitacao?> ObterStatusPorNomeAsync(string nome)
    {
        return _context.StatusSolicitacoes.FirstOrDefaultAsync(s => s.Nome == nome);
    }

    public Task<List<StatusSolicitacao>> ListarStatusAsync()
    {
        return _context.StatusSolicitacoes.AsNoTracking().OrderBy(s => s.Nome).ToListAsync();
    }

    public Task<List<TipoProgresso>> ListarTiposProgressoAsync()
    {
        return _context.TiposProgresso.AsNoTracking().OrderBy(t => t.Nome).ToListAsync();
    }

    public Task<List<Nivel>> ListarNiveisAsync()
    {
        return _context.Niveis.AsNoTracking().Where(n => n.Ativo).OrderBy(n => n.Ordem).ToListAsync();
    }

    public Task<Atividade?> ObterAtividadeAsync(int atividadeId, int solicitacaoProgressaoId)
    {
        return _context.Atividades
            .Include(a => a.SolicitacaoProgressao)
            .Include(a => a.AvaliacoesRevisores)
                .ThenInclude(ar => ar.SolicitacaoRevisor)
                    .ThenInclude(sr => sr.RevisorUsuario)
            .FirstOrDefaultAsync(a => a.AtividadeId == atividadeId
                && a.SolicitacaoProgressaoId == solicitacaoProgressaoId);
    }

    public Task<Documento?> ObterDocumentoAsync(int documentoId)
    {
        return _context.Documentos.AsNoTracking().FirstOrDefaultAsync(d => d.DocumentoId == documentoId);
    }

    public Task<SolicitacaoRevisor?> ObterSolicitacaoRevisorAsync(int solicitacaoProgressaoId, int revisorUsuarioId)
    {
        return _context.SolicitacoesRevisores.FirstOrDefaultAsync(sr =>
            sr.SolicitacaoProgressaoId == solicitacaoProgressaoId && sr.RevisorUsuarioId == revisorUsuarioId);
    }

    public async Task AdicionarSolicitacaoRevisorAsync(SolicitacaoRevisor solicitacaoRevisor)
    {
        await _context.SolicitacoesRevisores.AddAsync(solicitacaoRevisor);
    }

    public Task SalvarAlteracoesAsync()
    {
        return _context.SaveChangesAsync();
    }

    private IQueryable<SolicitacaoProgressao> QueryBase()
    {
        return _context.SolicitacoesProgressao
            .Include(s => s.Usuario)
                .ThenInclude(u => u.VinculosInstitucionais)
            .Include(s => s.StatusSolicitacao)
            .Include(s => s.TipoProgresso)
            .Include(s => s.NivelOrigem)
            .Include(s => s.NivelDestino)
            .Include(s => s.RevisorUsuario)
            .Include(s => s.AtribuidoPorUsuario)
            .Include(s => s.Revisores)
                .ThenInclude(sr => sr.RevisorUsuario)
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

    private static int CalcularPercentualPreenchimento(SolicitacaoProgressao solicitacao)
    {
        var percentual = 0;
        if (solicitacao.TipoProgressoId > 0 && solicitacao.NivelOrigemId > 0 && solicitacao.NivelDestinoId > 0)
            percentual += 35;
        if (!string.IsNullOrWhiteSpace(solicitacao.Observacao))
            percentual += 15;
        if (solicitacao.Atividades.Count > 0)
            percentual += 30;
        if (solicitacao.Documentos.Count > 0)
            percentual += 20;
        return Math.Min(percentual, 100);
    }
}
