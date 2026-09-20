using ProgressoAcademico.Application.DTOs.Admin;
using ProgressoAcademico.Application.DTOs.Atividades;
using ProgressoAcademico.Application.DTOs.Documentos;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Admin;

namespace ProgressoAcademico.Services.Revisor;

public class RevisorSolicitacaoService : IRevisorSolicitacaoService
{
    private readonly IRevisorSolicitacaoRepository _repository;

    public RevisorSolicitacaoService(IRevisorSolicitacaoRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdminDashboardDto> ObterDashboardAsync(int revisorUsuarioId)
    {
        var resumos = (await ListarAsync(revisorUsuarioId)).ToList();

        return new AdminDashboardDto
        {
            TotalSolicitacoes = resumos.Count,
            EmRevisao = resumos.Count(s => s.Status == StatusSolicitacaoNomes.EmRevisao),
            AguardandoDecisaoFinal = resumos.Count(s => s.Status == StatusSolicitacaoNomes.AguardandoDecisaoFinal),
            AjustesSolicitados = resumos.Count(s => s.Status == StatusSolicitacaoNomes.AjustesSolicitados),
            Aprovadas = resumos.Count(s => s.Status == StatusSolicitacaoNomes.Aprovada),
            Negadas = resumos.Count(s => s.Status == StatusSolicitacaoNomes.Negada),
            Encerradas = resumos.Count(s => s.Status == StatusSolicitacaoNomes.Encerrada),
            TotalAtividades = resumos.Sum(s => s.TotalAtividades),
            TotalDocumentos = resumos.Sum(s => s.TotalDocumentos),
            PercentualMedioPreenchimento = resumos.Count == 0
                ? 0
                : (int)Math.Round(resumos.Average(s => s.PercentualPreenchimento), MidpointRounding.AwayFromZero),
            Recentes = resumos.Take(6).ToList()
        };
    }

    public async Task<IReadOnlyList<AdminSolicitacaoResumoDto>> ListarAsync(int revisorUsuarioId)
    {
        var solicitacoes = await _repository.ListarAtribuidasAsync(revisorUsuarioId);
        return solicitacoes.Select(MapearResumo).ToList();
    }

    public async Task<AdminSolicitacaoDetalheDto?> ObterDetalheAsync(int solicitacaoProgressaoId, int revisorUsuarioId)
    {
        var solicitacao = await _repository.ObterDetalheAtribuidaAsync(solicitacaoProgressaoId, revisorUsuarioId);
        return solicitacao == null ? null : MapearDetalhe(solicitacao, revisorUsuarioId);
    }

    public async Task<AdminDecisaoResultadoDto> AvaliarAtividadeAsync(AdminAvaliarAtividadeViewModel model, int revisorUsuarioId)
    {
        var resultadosValidos = new[] { "Aceita", "Rejeitada", "SemEvidencia" };
        if (!resultadosValidos.Contains(model.Resultado))
            return AdminDecisaoResultadoDto.Falha("Resultado da avaliacao invalido.");

        var atividade = await _repository.ObterAtividadeAsync(model.AtividadeId, model.SolicitacaoProgressaoId, revisorUsuarioId);
        if (atividade == null)
            return AdminDecisaoResultadoDto.Falha("Atividade nao encontrada nas solicitacoes atribuidas ao revisor.");

        var atribuicao = await _repository.ObterAtribuicaoAsync(model.SolicitacaoProgressaoId, revisorUsuarioId);
        if (atribuicao == null)
            return AdminDecisaoResultadoDto.Falha("Solicitacao nao atribuida a este revisor.");

        var avaliacao = await _repository.ObterAvaliacaoAtividadeAsync(atividade.AtividadeId, atribuicao.SolicitacaoRevisorId);
        if (avaliacao == null)
        {
            avaliacao = new AtividadeAvaliacaoRevisor
            {
                AtividadeId = atividade.AtividadeId,
                SolicitacaoRevisorId = atribuicao.SolicitacaoRevisorId
            };
            await _repository.AdicionarAvaliacaoAtividadeAsync(avaliacao);
        }

        avaliacao.Resultado = model.Resultado;
        avaliacao.Parecer = model.Parecer.Trim();
        avaliacao.DataAvaliacao = DateTime.UtcNow;
        if (atividade.SolicitacaoProgressao != null)
            atividade.SolicitacaoProgressao.DataUltimaMovimentacao = DateTime.UtcNow;

        await _repository.SalvarAlteracoesAsync();
        return AdminDecisaoResultadoDto.Ok();
    }

    public Task<AdminDecisaoResultadoDto> DevolverParaAjustesAsync(int solicitacaoProgressaoId, int revisorUsuarioId, string parecer)
    {
        return RegistrarParecerAsync(solicitacaoProgressaoId, revisorUsuarioId, parecer, StatusRevisaoNomes.AjustesSolicitados);
    }

    public Task<AdminDecisaoResultadoDto> EncaminharParaAdmAsync(int solicitacaoProgressaoId, int revisorUsuarioId, string parecer)
    {
        return RegistrarParecerAsync(solicitacaoProgressaoId, revisorUsuarioId, parecer, StatusRevisaoNomes.EncaminhadaAoAdm);
    }

    public async Task<DocumentoDownloadDto?> ObterDocumentoAsync(int documentoId, int revisorUsuarioId)
    {
        var documento = await _repository.ObterDocumentoAsync(documentoId, revisorUsuarioId);
        return documento == null ? null : new DocumentoDownloadDto
        {
            NomeArquivo = documento.NomeArquivo,
            ContentType = documento.ContentType,
            Arquivo = documento.Arquivo
        };
    }

    private async Task<AdminDecisaoResultadoDto> RegistrarParecerAsync(
        int solicitacaoProgressaoId,
        int revisorUsuarioId,
        string parecer,
        string statusRevisao)
    {
        if (string.IsNullOrWhiteSpace(parecer))
            return AdminDecisaoResultadoDto.Falha("Informe o parecer da revisao.");

        var atribuicao = await _repository.ObterAtribuicaoAsync(solicitacaoProgressaoId, revisorUsuarioId);

        if (atribuicao == null)
            return AdminDecisaoResultadoDto.Falha("Solicitacao nao atribuida a este revisor.");

        if (StatusSolicitacaoNomes.EhFinal(atribuicao.SolicitacaoProgressao.StatusSolicitacao.Nome))
            return AdminDecisaoResultadoDto.Falha("Solicitacoes finalizadas nao podem receber parecer do revisor.");

        atribuicao.Parecer = parecer.Trim();
        atribuicao.DataParecer = DateTime.UtcNow;
        atribuicao.StatusRevisao = statusRevisao;

        var novoStatusNome = TodasRevisoesEncaminhadas(atribuicao.SolicitacaoProgressao)
            ? StatusSolicitacaoNomes.AguardandoDecisaoFinal
            : statusRevisao == StatusRevisaoNomes.AjustesSolicitados
                ? StatusSolicitacaoNomes.AjustesSolicitados
                : StatusSolicitacaoNomes.EmRevisao;

        var novoStatus = await _repository.ObterStatusPorNomeAsync(novoStatusNome);
        if (novoStatus == null)
            return AdminDecisaoResultadoDto.Falha("Status da solicitacao nao encontrado.");

        atribuicao.SolicitacaoProgressao.StatusSolicitacaoId = novoStatus.StatusSolicitacaoId;
        atribuicao.SolicitacaoProgressao.DataFechamento = null;
        atribuicao.SolicitacaoProgressao.DataUltimaMovimentacao = DateTime.UtcNow;

        await _repository.SalvarAlteracoesAsync();
        return AdminDecisaoResultadoDto.Ok();
    }

    private static AdminSolicitacaoDetalheDto MapearDetalhe(SolicitacaoProgressao solicitacao, int revisorUsuarioId)
    {
        var resumo = MapearResumo(solicitacao);
        return new AdminSolicitacaoDetalheDto
        {
            SolicitacaoProgressaoId = resumo.SolicitacaoProgressaoId,
            Professor = resumo.Professor,
            EmailProfessor = resumo.EmailProfessor,
            Status = resumo.Status,
            TipoProgressao = resumo.TipoProgressao,
            NivelOrigem = resumo.NivelOrigem,
            NivelDestino = resumo.NivelDestino,
            Multiprogressao = resumo.Multiprogressao,
            Revisor = resumo.Revisor,
            DataAtribuicaoRevisor = resumo.DataAtribuicaoRevisor,
            DataCriacao = resumo.DataCriacao,
            DataFechamento = resumo.DataFechamento,
            Apelido = resumo.Apelido,
            PercentualPreenchimento = resumo.PercentualPreenchimento,
            TotalAtividades = resumo.TotalAtividades,
            TotalDocumentos = resumo.TotalDocumentos,
            Observacao = solicitacao.Observacao,
            ParecerFinal = solicitacao.ParecerFinal,
            ParecerRevisor = solicitacao.ParecerRevisor,
            DataParecerRevisor = solicitacao.DataParecerRevisor,
            RevisadoPor = solicitacao.RevisadoPorUsuario?.Nome,
            AtribuidoPor = solicitacao.AtribuidoPorUsuario?.Nome,
            DataRevisao = solicitacao.DataRevisao,
            Atividades = solicitacao.Atividades.Select(a => MapearAtividade(a, revisorUsuarioId)).ToList(),
            Documentos = solicitacao.Documentos.Select(MapearDocumento).ToList(),
            Revisores = solicitacao.Revisores.Select(MapearRevisor).ToList()
        };
    }

    private static AdminSolicitacaoResumoDto MapearResumo(SolicitacaoProgressao solicitacao)
    {
        return new AdminSolicitacaoResumoDto
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            Professor = solicitacao.Usuario.Nome,
            EmailProfessor = solicitacao.Usuario.Email,
            Status = solicitacao.StatusSolicitacao.Nome,
            TipoProgressao = solicitacao.TipoProgresso.Nome,
            NivelOrigem = solicitacao.NivelOrigem.Codigo,
            NivelDestino = solicitacao.NivelDestino.Codigo,
            Multiprogressao = solicitacao.Multiprogressao,
            Revisor = ResumirRevisores(solicitacao),
            TotalRevisores = solicitacao.Revisores.Count,
            RevisoresPendentes = solicitacao.Revisores.Count(r => r.StatusRevisao != StatusRevisaoNomes.EncaminhadaAoAdm),
            DataAtribuicaoRevisor = solicitacao.DataAtribuicaoRevisor,
            DataCriacao = solicitacao.DataCriacao,
            DataFechamento = solicitacao.DataFechamento,
            Apelido = solicitacao.Apelido,
            PercentualPreenchimento = CalcularPercentualPreenchimento(solicitacao),
            TotalAtividades = solicitacao.Atividades.Count,
            TotalDocumentos = solicitacao.Documentos.Count
        };
    }

    private static bool TodasRevisoesEncaminhadas(SolicitacaoProgressao solicitacao)
    {
        return solicitacao.Revisores.Count > 0
            && solicitacao.Revisores.All(r => r.StatusRevisao == StatusRevisaoNomes.EncaminhadaAoAdm);
    }

    private static SolicitacaoRevisorDto MapearRevisor(SolicitacaoRevisor revisor)
    {
        return new SolicitacaoRevisorDto
        {
            SolicitacaoRevisorId = revisor.SolicitacaoRevisorId,
            RevisorUsuarioId = revisor.RevisorUsuarioId,
            Revisor = revisor.RevisorUsuario.Nome,
            Email = revisor.RevisorUsuario.Email,
            StatusRevisao = revisor.StatusRevisao,
            Parecer = revisor.Parecer,
            DataAtribuicao = revisor.DataAtribuicao,
            DataParecer = revisor.DataParecer
        };
    }

    private static string? ResumirRevisores(SolicitacaoProgressao solicitacao)
    {
        if (solicitacao.Revisores.Count == 0)
            return solicitacao.RevisorUsuario?.Nome;

        return string.Join(", ", solicitacao.Revisores.Select(r => r.RevisorUsuario.Nome));
    }

    private static AtividadeResumoDto MapearAtividade(
        Atividade atividade,
        int revisorUsuarioId)
    {
        var solicitacaoRevisorId = atividade.SolicitacaoProgressao?.Revisores
            .Where(r => r.RevisorUsuarioId == revisorUsuarioId)
            .Select(r => r.SolicitacaoRevisorId)
            .FirstOrDefault() ?? 0;

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
                .Where(a => a.SolicitacaoRevisorId == solicitacaoRevisorId)
                .OrderBy(a => a.DataAvaliacao)
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

    private static DocumentoResumoDto MapearDocumento(Documento documento)
    {
        return new DocumentoResumoDto
        {
            DocumentoId = documento.DocumentoId,
            SolicitacaoProgressaoId = documento.SolicitacaoProgressaoId,
            AtividadeId = documento.AtividadeId,
            TipoDocumento = documento.TipoDocumento.Tipo,
            NomeArquivo = documento.NomeArquivo,
            ContentType = documento.ContentType,
            TamanhoBytes = documento.TamanhoBytes,
            DataUpload = documento.DataUpload,
            OrigemDocumento = documento.OrigemDocumento,
            HashSha256 = documento.HashSha256,
            Observacao = documento.Observacao
        };
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
