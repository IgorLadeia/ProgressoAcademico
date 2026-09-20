using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressoAcademico.Application.DTOs.Atividades;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Atividades;

namespace ProgressoAcademico.Services.Atividades;

public class AtividadeService : IAtividadeService
{
    private readonly IAtividadeRepository _atividadeRepository;
    private readonly ISolicitacaoProgressaoRepository _solicitacaoRepository;

    public AtividadeService(
        IAtividadeRepository atividadeRepository,
        ISolicitacaoProgressaoRepository solicitacaoRepository)
    {
        _atividadeRepository = atividadeRepository;
        _solicitacaoRepository = solicitacaoRepository;
    }

    public async Task<IReadOnlyList<AtividadeResumoDto>> ListarPorSolicitacaoAsync(int usuarioId, int solicitacaoProgressaoId)
    {
        var atividades = await _atividadeRepository.ListarPorSolicitacaoAsync(solicitacaoProgressaoId, usuarioId);
        return atividades.Select(MapearResumo).ToList();
    }

    public async Task PreencherOpcoesAsync(AtividadeFormViewModel model)
    {
        var subtipos = await _atividadeRepository.ListarSubtiposAsync();

        model.SubtiposAtividade = subtipos
            .Select(s => new SelectListItem(
                $"{s.TipoAtividade.Nome} - {s.Nome} ({s.Pontos} pts)",
                s.SubtipoAtividadeId.ToString()))
            .ToList();
    }

    public async Task<CriarAtividadeResultadoDto> CriarAsync(int usuarioId, AtividadeFormViewModel model)
    {
        var solicitacao = await _solicitacaoRepository.ObterParaEdicaoPorIdEUsuarioAsync(model.SolicitacaoProgressaoId, usuarioId);

        if (solicitacao == null)
            return CriarAtividadeResultadoDto.Falha("Solicitacao nao encontrada.");

        if (!StatusSolicitacaoNomes.PermiteEdicaoProfessor(solicitacao.StatusSolicitacao.Nome))
            return CriarAtividadeResultadoDto.Falha("Solicitacoes bloqueadas nao permitem cadastro de atividades.");

        if (!model.SubTipoAtividadeId.HasValue)
            return CriarAtividadeResultadoDto.Falha("Selecione o subtipo de atividade.");

        var subtipo = await _atividadeRepository.ObterSubtipoAsync(model.SubTipoAtividadeId.Value);

        if (subtipo == null)
            return CriarAtividadeResultadoDto.Falha("Subtipo de atividade invalido.");

        if (!model.DataInicio.HasValue)
            return CriarAtividadeResultadoDto.Falha("Informe a data de inicio.");

        if (model.DataTermino.HasValue && model.DataTermino.Value.Date < model.DataInicio.Value.Date)
            return CriarAtividadeResultadoDto.Falha("A data de termino nao pode ser anterior a data de inicio.");

        var atividade = new Atividade
        {
            SolicitacaoProgressaoId = model.SolicitacaoProgressaoId,
            TipoAtividadeId = subtipo.TipoAtividadeId,
            SubTipoAtividadeId = subtipo.SubtipoAtividadeId,
            DataInicio = model.DataInicio.Value,
            DataTermino = model.DataTermino,
            Titulo = model.Titulo.Trim(),
            Descricao = string.IsNullOrWhiteSpace(model.Descricao) ? null : model.Descricao.Trim(),
            Quantidade = model.Quantidade,
            PontuacaoCalculada = model.Quantidade * subtipo.Pontos,
            Status = "Declarada",
            ParecerAvaliacao = null,
            DataAvaliacao = null
        };

        await ReabrirSeEmAjustesAsync(solicitacao);
        solicitacao.DataUltimaMovimentacao = DateTime.UtcNow;
        await _atividadeRepository.AdicionarAsync(atividade);
        await _atividadeRepository.SalvarAlteracoesAsync();

        return CriarAtividadeResultadoDto.Criada(atividade.AtividadeId);
    }

    public async Task<CriarAtividadeResultadoDto> AtualizarAsync(int usuarioId, AtividadeFormViewModel model)
    {
        if (!model.AtividadeId.HasValue)
            return CriarAtividadeResultadoDto.Falha("Atividade nao informada.");

        var atividade = await _atividadeRepository.ObterPorIdEUsuarioAsync(model.AtividadeId.Value, usuarioId);

        if (atividade == null || atividade.SolicitacaoProgressao == null)
            return CriarAtividadeResultadoDto.Falha("Atividade nao encontrada.");

        if (!StatusSolicitacaoNomes.PermiteEdicaoProfessor(atividade.SolicitacaoProgressao.StatusSolicitacao.Nome))
            return CriarAtividadeResultadoDto.Falha("Solicitacoes bloqueadas nao permitem alteracao de atividades.");

        if (!model.SubTipoAtividadeId.HasValue)
            return CriarAtividadeResultadoDto.Falha("Selecione o subtipo de atividade.");

        var subtipo = await _atividadeRepository.ObterSubtipoAsync(model.SubTipoAtividadeId.Value);

        if (subtipo == null)
            return CriarAtividadeResultadoDto.Falha("Subtipo de atividade invalido.");

        if (!model.DataInicio.HasValue)
            return CriarAtividadeResultadoDto.Falha("Informe a data de inicio.");

        if (model.DataTermino.HasValue && model.DataTermino.Value.Date < model.DataInicio.Value.Date)
            return CriarAtividadeResultadoDto.Falha("A data de termino nao pode ser anterior a data de inicio.");

        atividade.TipoAtividadeId = subtipo.TipoAtividadeId;
        atividade.SubTipoAtividadeId = subtipo.SubtipoAtividadeId;
        atividade.DataInicio = model.DataInicio.Value;
        atividade.DataTermino = model.DataTermino;
        atividade.Titulo = model.Titulo.Trim();
        atividade.Descricao = string.IsNullOrWhiteSpace(model.Descricao) ? null : model.Descricao.Trim();
        atividade.Quantidade = model.Quantidade;
        atividade.PontuacaoCalculada = model.Quantidade * subtipo.Pontos;
        atividade.Status = "Declarada";
        atividade.ParecerAvaliacao = null;
        atividade.DataAvaliacao = null;

        await ReabrirSeEmAjustesAsync(atividade.SolicitacaoProgressao);
        atividade.SolicitacaoProgressao.DataUltimaMovimentacao = DateTime.UtcNow;
        await _atividadeRepository.SalvarAlteracoesAsync();

        return CriarAtividadeResultadoDto.Criada(atividade.AtividadeId);
    }

    public async Task<bool> ExcluirAsync(int usuarioId, int atividadeId)
    {
        var atividade = await _atividadeRepository.ObterPorIdEUsuarioAsync(atividadeId, usuarioId);

        if (atividade == null || atividade.SolicitacaoProgressao == null
            || !StatusSolicitacaoNomes.PermiteEdicaoProfessor(atividade.SolicitacaoProgressao.StatusSolicitacao.Nome))
            return false;

        await ReabrirSeEmAjustesAsync(atividade.SolicitacaoProgressao);
        atividade.SolicitacaoProgressao.DataUltimaMovimentacao = DateTime.UtcNow;
        _atividadeRepository.Remover(atividade);
        await _atividadeRepository.SalvarAlteracoesAsync();

        return true;
    }

    private static AtividadeResumoDto MapearResumo(Atividade atividade)
    {
        return new AtividadeResumoDto
        {
            AtividadeId = atividade.AtividadeId,
            SolicitacaoProgressaoId = atividade.SolicitacaoProgressaoId,
            SubTipoAtividadeId = atividade.SubTipoAtividadeId,
            TipoAtividade = atividade.TipoAtividade?.Nome ?? string.Empty,
            SubtipoAtividade = atividade.SubTipoAtividade?.Nome ?? string.Empty,
            Titulo = atividade.Titulo,
            Descricao = atividade.Descricao,
            DataInicio = atividade.DataInicio,
            DataTermino = atividade.DataTermino,
            Quantidade = atividade.Quantidade,
            PontuacaoCalculada = atividade.PontuacaoCalculada,
            Status = atividade.Status,
            OrigemCadastro = atividade.OrigemCadastro,
            ParecerAvaliacao = atividade.ParecerAvaliacao,
            DataAvaliacao = atividade.DataAvaliacao,
            AvaliacoesRevisores = atividade.AvaliacoesRevisores
                .OrderBy(a => a.SolicitacaoRevisor.RevisorUsuario.Nome)
                .Select(MapearAvaliacaoRevisor)
                .ToList()
        };
    }

    private static AtividadeAvaliacaoRevisorDto MapearAvaliacaoRevisor(AtividadeAvaliacaoRevisor avaliacao)
    {
        return new AtividadeAvaliacaoRevisorDto
        {
            AtividadeAvaliacaoRevisorId = avaliacao.AtividadeAvaliacaoRevisorId,
            AtividadeId = avaliacao.AtividadeId,
            SolicitacaoRevisorId = avaliacao.SolicitacaoRevisorId,
            Revisor = avaliacao.SolicitacaoRevisor.RevisorUsuario.Nome,
            EmailRevisor = avaliacao.SolicitacaoRevisor.RevisorUsuario.Email,
            Resultado = avaliacao.Resultado,
            Parecer = avaliacao.Parecer,
            DataAvaliacao = avaliacao.DataAvaliacao
        };
    }

    private async Task ReabrirSeEmAjustesAsync(SolicitacaoProgressao solicitacao)
    {
        if (solicitacao.StatusSolicitacao.Nome != StatusSolicitacaoNomes.AjustesSolicitados)
            return;

        var statusEmProcessamento = await _solicitacaoRepository.ObterStatusPorNomeAsync(StatusSolicitacaoNomes.EmProcessamento);

        if (statusEmProcessamento == null)
            return;

        solicitacao.StatusSolicitacaoId = statusEmProcessamento.StatusSolicitacaoId;
        solicitacao.DataFechamento = null;
    }
}
