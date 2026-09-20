using ProgressoAcademico.Application.DTOs.Solicitacoes;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Services.Solicitacoes;

public class SolicitacaoProgressaoService : ISolicitacaoProgressaoService
{
    private readonly ISolicitacaoProgressaoRepository _solicitacaoRepository;

    public SolicitacaoProgressaoService(ISolicitacaoProgressaoRepository solicitacaoRepository)
    {
        _solicitacaoRepository = solicitacaoRepository;
    }

    public async Task<IReadOnlyList<SolicitacaoResumoDto>> ListarPorUsuarioAsync(int usuarioId)
    {
        var solicitacoes = await _solicitacaoRepository.ListarPorUsuarioAsync(usuarioId);
        return solicitacoes
            .Select(MapearResumo)
            .OrderByDescending(s => s.DataUltimaMovimentacao)
            .ThenByDescending(s => s.DataCriacao)
            .ToList();
    }

    public async Task<SolicitacaoDetalheDto?> ObterDetalheAsync(int solicitacaoProgressaoId)
    {
        var solicitacao = await _solicitacaoRepository.ObterDetalhePorIdAsync(solicitacaoProgressaoId);

        if (solicitacao == null)
            return null;

        return MapearDetalhe(solicitacao);
    }

    public async Task<SolicitacaoDetalheDto?> ObterDetalheDoProfessorAsync(int usuarioId, int solicitacaoProgressaoId)
    {
        var solicitacao = await _solicitacaoRepository.ObterDetalhePorIdEUsuarioAsync(solicitacaoProgressaoId, usuarioId);

        if (solicitacao == null)
            return null;

        return MapearDetalhe(solicitacao);
    }

    public async Task<ProfessorDashboardDto> ObterDashboardProfessorAsync(int usuarioId)
    {
        var solicitacoes = await ListarPorUsuarioAsync(usuarioId);
        var recentes = solicitacoes.Take(5).ToList();

        return new ProfessorDashboardDto
        {
            TotalSolicitacoes = solicitacoes.Count,
            EmProcessamento = solicitacoes.Count(s => s.Status == StatusSolicitacaoNomes.EmProcessamento),
            AjustesSolicitados = solicitacoes.Count(s => s.Status == StatusSolicitacaoNomes.AjustesSolicitados),
            Aprovadas = solicitacoes.Count(s => s.Status == StatusSolicitacaoNomes.Aprovada),
            Negadas = solicitacoes.Count(s => s.Status == StatusSolicitacaoNomes.Negada),
            Encerradas = solicitacoes.Count(s => s.Status == StatusSolicitacaoNomes.Encerrada),
            SolicitacoesRecentes = recentes
        };
    }

    public async Task<CriarSolicitacaoResultadoDto> CriarAsync(int usuarioId, CriarSolicitacaoDto dto)
    {
        if (await _solicitacaoRepository.ExisteSolicitacaoAbertaAsync(usuarioId))
            return CriarSolicitacaoResultadoDto.Falha("Voce ja possui uma solicitacao em aberto. Finalize ou aguarde o encerramento antes de criar outra.");

        if (dto.NivelOrigemId == dto.NivelDestinoId)
            return CriarSolicitacaoResultadoDto.Falha("O nivel de origem deve ser diferente do nivel de destino.");

        if (!await _solicitacaoRepository.TipoProgressoExisteAsync(dto.TipoProgressoId))
            return CriarSolicitacaoResultadoDto.Falha("Tipo de progressao invalido.");

        if (!await _solicitacaoRepository.NivelExisteAsync(dto.NivelOrigemId))
            return CriarSolicitacaoResultadoDto.Falha("Nivel de origem invalido.");

        if (!await _solicitacaoRepository.NivelExisteAsync(dto.NivelDestinoId))
            return CriarSolicitacaoResultadoDto.Falha("Nivel de destino invalido.");

        var statusInicial = await _solicitacaoRepository.ObterStatusPorNomeAsync(StatusSolicitacaoNomes.EmProcessamento);

        if (statusInicial == null)
            return CriarSolicitacaoResultadoDto.Falha("Status inicial nao encontrado no banco de dados.");

        var agora = DateTime.UtcNow;

        var solicitacao = new SolicitacaoProgressao
        {
            UsuarioId = usuarioId,
            StatusSolicitacaoId = statusInicial.StatusSolicitacaoId,
            TipoProgressoId = dto.TipoProgressoId,
            NivelOrigemId = dto.NivelOrigemId,
            NivelDestinoId = dto.NivelDestinoId,
            DataCriacao = agora,
            DataUltimaMovimentacao = agora,
            DataFechamento = null,
            Observacao = string.IsNullOrWhiteSpace(dto.Observacao) ? null : dto.Observacao.Trim(),
            ParecerFinal = null,
            Apelido = string.IsNullOrWhiteSpace(dto.Apelido) ? null : dto.Apelido.Trim()
        };

        await _solicitacaoRepository.AdicionarAsync(solicitacao);
        await _solicitacaoRepository.SalvarAlteracoesAsync();

        return CriarSolicitacaoResultadoDto.Criada(solicitacao.SolicitacaoProgressaoId);
    }

    public async Task<SubmeterSolicitacaoResultadoDto> SubmeterAsync(int usuarioId, int solicitacaoProgressaoId)
    {
        var solicitacao = await _solicitacaoRepository.ObterParaSubmissaoPorIdEUsuarioAsync(solicitacaoProgressaoId, usuarioId);

        if (solicitacao == null)
            return SubmeterSolicitacaoResultadoDto.Falha("Solicitacao nao encontrada.");

        if (!StatusSolicitacaoNomes.PermiteSubmissaoProfessor(solicitacao.StatusSolicitacao.Nome))
            return SubmeterSolicitacaoResultadoDto.Falha("Esta solicitacao nao esta aberta para submissao pelo professor.");

        if (solicitacao.Atividades.Count == 0)
            return SubmeterSolicitacaoResultadoDto.Falha("Cadastre ao menos uma atividade antes de submeter a solicitacao.");

        var novoStatusNome = solicitacao.Revisores.Count == 0
            ? StatusSolicitacaoNomes.AguardandoAtribuicao
            : StatusSolicitacaoNomes.EmRevisao;

        var novoStatus = await _solicitacaoRepository.ObterStatusPorNomeAsync(novoStatusNome);

        if (novoStatus == null)
            return SubmeterSolicitacaoResultadoDto.Falha("Status de submissao nao encontrado no banco de dados.");

        solicitacao.StatusSolicitacaoId = novoStatus.StatusSolicitacaoId;
        solicitacao.DataFechamento = null;
        solicitacao.DataUltimaMovimentacao = DateTime.UtcNow;

        foreach (var revisor in solicitacao.Revisores)
        {
            revisor.StatusRevisao = StatusRevisaoNomes.EmRevisao;
        }

        await _solicitacaoRepository.SalvarAlteracoesAsync();
        return SubmeterSolicitacaoResultadoDto.Ok();
    }

    private static SolicitacaoDetalheDto MapearDetalhe(SolicitacaoProgressao solicitacao)
    {
        var resumo = MapearResumo(solicitacao);

        return new SolicitacaoDetalheDto
        {
            SolicitacaoProgressaoId = resumo.SolicitacaoProgressaoId,
            Status = resumo.Status,
            TipoProgressao = resumo.TipoProgressao,
            NivelOrigem = resumo.NivelOrigem,
            NivelDestino = resumo.NivelDestino,
            DataCriacao = resumo.DataCriacao,
            DataUltimaMovimentacao = resumo.DataUltimaMovimentacao,
            DataFechamento = resumo.DataFechamento,
            Apelido = resumo.Apelido,
            Observacao = solicitacao.Observacao,
            ParecerFinal = solicitacao.ParecerFinal,
            TotalAtividades = solicitacao.Atividades.Count,
            TotalDocumentos = solicitacao.Documentos.Count,
            PodeEditar = SolicitacaoEditavel(solicitacao),
            PodeSubmeter = StatusSolicitacaoNomes.PermiteSubmissaoProfessor(solicitacao.StatusSolicitacao.Nome),
            PercentualPreenchimento = CalcularPercentualPreenchimento(solicitacao)
        };
    }

    private static SolicitacaoResumoDto MapearResumo(SolicitacaoProgressao solicitacao)
    {
        return new SolicitacaoResumoDto
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            Status = solicitacao.StatusSolicitacao.Nome,
            TipoProgressao = solicitacao.TipoProgresso.Nome,
            NivelOrigem = solicitacao.NivelOrigem.Codigo,
            NivelDestino = solicitacao.NivelDestino.Codigo,
            DataCriacao = solicitacao.DataCriacao,
            DataUltimaMovimentacao = solicitacao.DataUltimaMovimentacao ?? solicitacao.DataCriacao,
            DataFechamento = solicitacao.DataFechamento,
            Apelido = solicitacao.Apelido,
            TotalAtividades = solicitacao.Atividades.Count,
            TotalDocumentos = solicitacao.Documentos.Count,
            PodeEditar = SolicitacaoEditavel(solicitacao),
            PodeSubmeter = StatusSolicitacaoNomes.PermiteSubmissaoProfessor(solicitacao.StatusSolicitacao.Nome),
            PercentualPreenchimento = CalcularPercentualPreenchimento(solicitacao)
        };
    }

    private static bool SolicitacaoEditavel(SolicitacaoProgressao solicitacao)
    {
        return StatusSolicitacaoNomes.PermiteEdicaoProfessor(solicitacao.StatusSolicitacao.Nome);
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
