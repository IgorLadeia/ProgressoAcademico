using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressoAcademico.Application.DTOs.Admin;
using ProgressoAcademico.Application.DTOs.Atividades;
using ProgressoAcademico.Application.DTOs.Documentos;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Admin;
using ProgressoAcademico.Services.Elegibilidade;

namespace ProgressoAcademico.Services.Admin;

public class AdminSolicitacaoService : IAdminSolicitacaoService
{
    private readonly IAdminSolicitacaoRepository _adminRepository;
    private readonly IElegibilidadeService _elegibilidadeService;

    public AdminSolicitacaoService(
        IAdminSolicitacaoRepository adminRepository,
        IElegibilidadeService elegibilidadeService)
    {
        _adminRepository = adminRepository;
        _elegibilidadeService = elegibilidadeService;
    }

    public async Task<AdminDashboardDto> ObterDashboardAsync()
    {
        var solicitacoes = await _adminRepository.ListarAsync(new AdminSolicitacaoFiltroDto());
        var resumos = solicitacoes.Select(MapearResumo).ToList();

        return new AdminDashboardDto
        {
            TotalSolicitacoes = resumos.Count,
            AguardandoAtribuicao = resumos.Count(s => s.Status == StatusSolicitacaoNomes.AguardandoAtribuicao),
            EmProcessamento = resumos.Count(s => s.Status == StatusSolicitacaoNomes.EmProcessamento),
            EmRevisao = resumos.Count(s => s.Status == StatusSolicitacaoNomes.EmRevisao),
            AguardandoDecisaoFinal = resumos.Count(s => s.Status == StatusSolicitacaoNomes.AguardandoDecisaoFinal),
            AjustesSolicitados = resumos.Count(s => s.Status == StatusSolicitacaoNomes.AjustesSolicitados),
            Aprovadas = resumos.Count(s => s.Status == StatusSolicitacaoNomes.Aprovada),
            Negadas = resumos.Count(s => s.Status == StatusSolicitacaoNomes.Negada),
            Encerradas = resumos.Count(s => s.Status == StatusSolicitacaoNomes.Encerrada),
            TotalProfessores = await _adminRepository.ContarProfessoresAsync(),
            TotalRevisores = await _adminRepository.ContarRevisoresAsync(),
            TotalDocumentos = await _adminRepository.ContarDocumentosAsync(),
            TotalAtividades = await _adminRepository.ContarAtividadesAsync(),
            PercentualMedioPreenchimento = resumos.Count == 0
                ? 0
                : (int)Math.Round(resumos.Average(s => s.PercentualPreenchimento), MidpointRounding.AwayFromZero),
            Recentes = resumos.Take(6).ToList()
        };
    }

    public async Task<IReadOnlyList<AdminSolicitacaoResumoDto>> ListarAsync(AdminSolicitacaoFiltroDto filtro)
    {
        var solicitacoes = await _adminRepository.ListarAsync(filtro);
        return solicitacoes.Select(MapearResumo).ToList();
    }

    public async Task<AdminSolicitacaoDetalheDto?> ObterDetalheAsync(int solicitacaoProgressaoId)
    {
        var solicitacao = await _adminRepository.ObterDetalheAsync(solicitacaoProgressaoId);

        if (solicitacao == null)
            return null;

        var detalhe = MapearDetalhe(solicitacao);
        detalhe.Elegibilidade = await _elegibilidadeService.AvaliarAsync(solicitacao.UsuarioId, solicitacao.SolicitacaoProgressaoId);
        return detalhe;
    }

    public async Task PreencherFiltrosAsync(AdminSolicitacaoFiltroViewModel filtro)
    {
        filtro.StatusOpcoes = (await _adminRepository.ListarStatusAsync())
            .Select(s => new SelectListItem(s.Nome, s.Nome))
            .ToList();

        filtro.TiposProgresso = (await _adminRepository.ListarTiposProgressoAsync())
            .Select(t => new SelectListItem(t.Nome, t.TipoProgressoId.ToString()))
            .ToList();
    }

    public async Task<AdminCorrigirSolicitacaoViewModel?> ObterParaCorrecaoAsync(int solicitacaoProgressaoId)
    {
        var solicitacao = await _adminRepository.ObterDetalheAsync(solicitacaoProgressaoId);

        if (solicitacao == null)
            return null;

        var model = new AdminCorrigirSolicitacaoViewModel
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            TipoProgressoId = solicitacao.TipoProgressoId,
            NivelOrigemId = solicitacao.NivelOrigemId,
            NivelDestinoId = solicitacao.NivelDestinoId,
            Status = solicitacao.StatusSolicitacao.Nome,
            Apelido = solicitacao.Apelido,
            Observacao = solicitacao.Observacao,
            ParecerFinal = solicitacao.ParecerFinal
        };

        await PreencherCorrecaoAsync(model);
        return model;
    }

    public async Task PreencherCorrecaoAsync(AdminCorrigirSolicitacaoViewModel model)
    {
        model.StatusOpcoes = (await _adminRepository.ListarStatusAsync())
            .Select(s => new SelectListItem(s.Nome, s.Nome))
            .ToList();

        model.TiposProgresso = (await _adminRepository.ListarTiposProgressoAsync())
            .Select(t => new SelectListItem(t.Nome, t.TipoProgressoId.ToString()))
            .ToList();

        model.Niveis = (await _adminRepository.ListarNiveisAsync())
            .Select(n => new SelectListItem($"{n.Codigo} - {n.Classe}", n.NivelId.ToString()))
            .ToList();
    }

    public async Task<AdminAtribuirRevisorViewModel?> ObterParaAtribuicaoAsync(int solicitacaoProgressaoId)
    {
        var solicitacao = await _adminRepository.ObterDetalheAsync(solicitacaoProgressaoId);

        if (solicitacao == null)
            return null;

        var model = new AdminAtribuirRevisorViewModel
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            Professor = solicitacao.Usuario.Nome,
            StatusAtual = solicitacao.StatusSolicitacao.Nome,
            RevisorUsuarioIds = solicitacao.Revisores.Select(r => r.RevisorUsuarioId).ToList(),
            RevisoresAtribuidos = solicitacao.Revisores.Select(MapearRevisor).ToList()
        };

        await PreencherAtribuicaoAsync(model);
        return model;
    }

    public async Task PreencherAtribuicaoAsync(AdminAtribuirRevisorViewModel model)
    {
        model.Revisores = (await _adminRepository.ListarRevisoresAsync())
            .Select(r => new SelectListItem($"{r.Nome} ({r.Email})", r.UsuarioId.ToString()))
            .ToList();
    }

    public async Task<AdminDecisaoResultadoDto> AtribuirRevisorAsync(AdminAtribuirRevisorViewModel model, int adminId)
    {
        var revisorIds = model.RevisorUsuarioIds.Distinct().ToList();
        if (revisorIds.Count == 0)
            return AdminDecisaoResultadoDto.Falha("Selecione ao menos um professor revisor.");

        var solicitacao = await _adminRepository.ObterDetalheAsync(model.SolicitacaoProgressaoId);
        var revisores = await _adminRepository.ObterUsuariosAsync(revisorIds);
        var status = await _adminRepository.ObterStatusPorNomeAsync(StatusSolicitacaoNomes.EmRevisao);

        if (solicitacao == null || status == null)
            return AdminDecisaoResultadoDto.Falha("Solicitacao ou status nao encontrado.");

        if (!StatusSolicitacaoNomes.VisivelParaAdministracao(solicitacao.StatusSolicitacao.Nome))
            return AdminDecisaoResultadoDto.Falha("A solicitacao ainda nao foi submetida pelo professor.");

        if (revisores.Count != revisorIds.Count || revisores.Any(r => !r.Ativo || !r.PodeRevisar))
            return AdminDecisaoResultadoDto.Falha("Selecione apenas professores com funcao de revisao ativa.");

        if (StatusSolicitacaoNomes.EhFinal(solicitacao.StatusSolicitacao.Nome))
            return AdminDecisaoResultadoDto.Falha("Solicitacoes finalizadas nao podem ser atribuidas para revisao.");

        foreach (var revisor in revisores)
        {
            var existente = await _adminRepository.ObterSolicitacaoRevisorAsync(solicitacao.SolicitacaoProgressaoId, revisor.UsuarioId);
            if (existente != null)
                continue;

            await _adminRepository.AdicionarSolicitacaoRevisorAsync(new SolicitacaoRevisor
            {
                SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
                RevisorUsuarioId = revisor.UsuarioId,
                AtribuidoPorUsuarioId = adminId,
                DataAtribuicao = DateTime.UtcNow,
                StatusRevisao = StatusRevisaoNomes.EmRevisao
            });
        }

        solicitacao.StatusSolicitacaoId = status.StatusSolicitacaoId;
        solicitacao.DataFechamento = null;
        solicitacao.DataUltimaMovimentacao = DateTime.UtcNow;

        await _adminRepository.SalvarAlteracoesAsync();
        return AdminDecisaoResultadoDto.Ok();
    }

    public async Task<AdminDecisaoResultadoDto> CorrigirAsync(AdminCorrigirSolicitacaoViewModel model)
    {
        if (model.NivelOrigemId == model.NivelDestinoId)
            return AdminDecisaoResultadoDto.Falha("Nivel origem e destino devem ser diferentes.");

        var solicitacao = await _adminRepository.ObterDetalheAsync(model.SolicitacaoProgressaoId);
        var status = await _adminRepository.ObterStatusPorNomeAsync(model.Status);

        if (solicitacao == null || status == null)
            return AdminDecisaoResultadoDto.Falha("Solicitacao ou status nao encontrado.");

        solicitacao.TipoProgressoId = model.TipoProgressoId;
        solicitacao.NivelOrigemId = model.NivelOrigemId;
        solicitacao.NivelDestinoId = model.NivelDestinoId;
        solicitacao.StatusSolicitacaoId = status.StatusSolicitacaoId;
        solicitacao.Apelido = string.IsNullOrWhiteSpace(model.Apelido) ? null : model.Apelido.Trim();
        solicitacao.Observacao = string.IsNullOrWhiteSpace(model.Observacao) ? null : model.Observacao.Trim();
        solicitacao.ParecerFinal = string.IsNullOrWhiteSpace(model.ParecerFinal) ? null : model.ParecerFinal.Trim();
        solicitacao.DataFechamento = StatusSolicitacaoNomes.EhFinal(status.Nome) ? DateTime.UtcNow : null;
        solicitacao.DataUltimaMovimentacao = DateTime.UtcNow;

        await _adminRepository.SalvarAlteracoesAsync();
        return AdminDecisaoResultadoDto.Ok();
    }

    public Task<AdminDecisaoResultadoDto> AprovarAsync(int solicitacaoProgressaoId, int adminId, string parecer)
    {
        return AplicarDecisaoAsync(solicitacaoProgressaoId, adminId, parecer, StatusSolicitacaoNomes.Aprovada);
    }

    public Task<AdminDecisaoResultadoDto> ReprovarAsync(int solicitacaoProgressaoId, int adminId, string parecer)
    {
        return AplicarDecisaoAsync(solicitacaoProgressaoId, adminId, parecer, StatusSolicitacaoNomes.Negada);
    }

    public Task<AdminDecisaoResultadoDto> SolicitarAjustesAsync(int solicitacaoProgressaoId, int adminId, string parecer)
    {
        return AplicarDecisaoAsync(solicitacaoProgressaoId, adminId, parecer, StatusSolicitacaoNomes.AjustesSolicitados);
    }

    public Task<AdminDecisaoResultadoDto> EncerrarAsync(int solicitacaoProgressaoId, int adminId, string parecer)
    {
        return AplicarDecisaoAsync(solicitacaoProgressaoId, adminId, parecer, StatusSolicitacaoNomes.Encerrada);
    }

    public async Task<AdminDecisaoResultadoDto> AvaliarAtividadeAsync(AdminAvaliarAtividadeViewModel model)
    {
        var resultadosValidos = new[] { "Aceita", "Rejeitada", "SemEvidencia" };
        if (!resultadosValidos.Contains(model.Resultado))
            return AdminDecisaoResultadoDto.Falha("Resultado da avaliação inválido.");

        var atividade = await _adminRepository.ObterAtividadeAsync(model.AtividadeId, model.SolicitacaoProgressaoId);
        if (atividade == null)
            return AdminDecisaoResultadoDto.Falha("Atividade não encontrada nesta solicitação.");

        atividade.Status = model.Resultado;
        atividade.ParecerAvaliacao = model.Parecer.Trim();
        atividade.DataAvaliacao = DateTime.UtcNow;
        if (atividade.SolicitacaoProgressao != null)
            atividade.SolicitacaoProgressao.DataUltimaMovimentacao = DateTime.UtcNow;
        await _adminRepository.SalvarAlteracoesAsync();
        return AdminDecisaoResultadoDto.Ok();
    }

    public async Task<DocumentoDownloadDto?> ObterDocumentoAsync(int documentoId)
    {
        var documento = await _adminRepository.ObterDocumentoAsync(documentoId);
        return documento == null ? null : new DocumentoDownloadDto
        {
            NomeArquivo = documento.NomeArquivo,
            ContentType = documento.ContentType,
            Arquivo = documento.Arquivo
        };
    }

    private async Task<AdminDecisaoResultadoDto> AplicarDecisaoAsync(int solicitacaoProgressaoId, int adminId, string parecer, string statusNome)
    {
        if (string.IsNullOrWhiteSpace(parecer))
            return AdminDecisaoResultadoDto.Falha("Parecer administrativo e obrigatorio.");

        var solicitacao = await _adminRepository.ObterDetalheAsync(solicitacaoProgressaoId);
        var status = await _adminRepository.ObterStatusPorNomeAsync(statusNome);

        if (solicitacao == null || status == null)
            return AdminDecisaoResultadoDto.Falha("Solicitacao ou status nao encontrado.");

        if (!StatusSolicitacaoNomes.VisivelParaAdministracao(solicitacao.StatusSolicitacao.Nome))
            return AdminDecisaoResultadoDto.Falha("A solicitacao ainda nao foi submetida pelo professor.");

        solicitacao.StatusSolicitacaoId = status.StatusSolicitacaoId;
        solicitacao.ParecerFinal = parecer.Trim();
        solicitacao.RevisadoPorUsuarioId = adminId;
        solicitacao.DataRevisao = DateTime.UtcNow;
        solicitacao.DataFechamento = StatusSolicitacaoNomes.EhFinal(statusNome) ? DateTime.UtcNow : null;
        solicitacao.DataUltimaMovimentacao = DateTime.UtcNow;

        if (statusNome == StatusSolicitacaoNomes.Aprovada)
            AtualizarDadosProfessorAposAprovacao(solicitacao, solicitacao.DataFechamento ?? DateTime.UtcNow);

        await _adminRepository.SalvarAlteracoesAsync();
        return AdminDecisaoResultadoDto.Ok();
    }

    private static void AtualizarDadosProfessorAposAprovacao(SolicitacaoProgressao solicitacao, DateTime dataAprovacao)
    {
        solicitacao.Usuario.DataUltimoProgresso = dataAprovacao;

        var vinculoAtivo = solicitacao.Usuario.VinculosInstitucionais.FirstOrDefault(v => v.Ativo);

        if (vinculoAtivo == null)
            return;

        vinculoAtivo.NivelId = solicitacao.NivelDestinoId;
        vinculoAtivo.DataUltimaProgressao = dataAprovacao;
    }

    private static AdminSolicitacaoDetalheDto MapearDetalhe(SolicitacaoProgressao solicitacao)
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
            Revisor = resumo.Revisor,
            AtribuidoPor = solicitacao.AtribuidoPorUsuario?.Nome,
            DataAtribuicaoRevisor = resumo.DataAtribuicaoRevisor,
            Multiprogressao = resumo.Multiprogressao,
            DataRevisao = solicitacao.DataRevisao,
            Atividades = solicitacao.Atividades.Select(MapearAtividade).ToList(),
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

    private static AtividadeResumoDto MapearAtividade(Atividade atividade)
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
