using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressoAcademico.Application.DTOs.Documentos;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Documentos;
using System.Security.Cryptography;

namespace ProgressoAcademico.Services.Documentos;

public class DocumentoService : IDocumentoService
{
    private const long TamanhoMaximoPdfBytes = 10 * 1024 * 1024;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ISolicitacaoProgressaoRepository _solicitacaoRepository;
    private readonly IAtividadeRepository _atividadeRepository;

    public DocumentoService(
        IDocumentoRepository documentoRepository,
        ISolicitacaoProgressaoRepository solicitacaoRepository,
        IAtividadeRepository atividadeRepository)
    {
        _documentoRepository = documentoRepository;
        _solicitacaoRepository = solicitacaoRepository;
        _atividadeRepository = atividadeRepository;
    }

    public async Task<IReadOnlyList<DocumentoResumoDto>> ListarPorSolicitacaoAsync(int usuarioId, int solicitacaoProgressaoId)
    {
        var documentos = await _documentoRepository.ListarPorSolicitacaoAsync(solicitacaoProgressaoId, usuarioId);
        return documentos.Select(MapearResumo).ToList();
    }

    public async Task PreencherOpcoesUploadAsync(DocumentoUploadViewModel model)
    {
        var tipos = await _documentoRepository.ListarTiposDocumentoAsync();
        model.TiposDocumento = tipos
            .Select(t => new SelectListItem(t.Tipo, t.TipoDocumentoId.ToString()))
            .ToList();

        model.OpcoesOrigensDocumento = new List<SelectListItem>
        {
            new("Comprovatorio do professor", OrigensDocumento.ComprovatorioProfessor),
            new("Documento oficial da universidade", OrigensDocumento.DocumentoOficialUfabc)
        };
    }

    public async Task<UploadDocumentoResultadoDto> EnviarAsync(int usuarioId, DocumentoUploadViewModel model)
    {
        var solicitacao = await _solicitacaoRepository.ObterParaEdicaoPorIdEUsuarioAsync(model.SolicitacaoProgressaoId, usuarioId);

        if (solicitacao == null)
            return UploadDocumentoResultadoDto.Falha("Solicitacao nao encontrada.");

        if (!SolicitacaoEditavel(solicitacao))
            return UploadDocumentoResultadoDto.Falha("Solicitacoes finalizadas nao permitem novos anexos.");

        if (!OrigensDocumento.EhValida(model.OrigemDocumento))
            return UploadDocumentoResultadoDto.Falha("Origem do documento invalida.");

        var tipoDocumentoId = await ResolverTipoDocumentoIdAsync(model);
        if (!tipoDocumentoId.HasValue)
            return UploadDocumentoResultadoDto.Falha("Tipo de documento invalido.");

        if (model.OrigemDocumento == OrigensDocumento.ComprovatorioProfessor && !model.AtividadeId.HasValue)
            return UploadDocumentoResultadoDto.Falha("Selecione a atividade relacionada ao comprovante.");

        if (model.OrigemDocumento != OrigensDocumento.ComprovatorioProfessor && model.AtividadeId.HasValue)
            return UploadDocumentoResultadoDto.Falha("O documento institucional deve ficar vinculado apenas a solicitacao.");

        Atividade? atividadeRelacionada = null;
        if (model.AtividadeId.HasValue)
        {
            atividadeRelacionada = await _atividadeRepository.ObterPorIdEUsuarioAsync(model.AtividadeId.Value, usuarioId);
            if (atividadeRelacionada?.SolicitacaoProgressaoId != model.SolicitacaoProgressaoId)
                return UploadDocumentoResultadoDto.Falha("Atividade invalida para esta solicitacao.");
        }

        if (model.Arquivo == null || model.Arquivo.Length == 0)
            return UploadDocumentoResultadoDto.Falha("Arquivo nao informado.");

        if (model.Arquivo.Length > TamanhoMaximoPdfBytes)
            return UploadDocumentoResultadoDto.Falha("O arquivo deve ter no maximo 10 MB.");

        if (!ArquivoPdf(model.Arquivo))
            return UploadDocumentoResultadoDto.Falha("Apenas arquivos PDF sao aceitos nesta etapa.");

        await using var stream = model.Arquivo.OpenReadStream();
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        var bytes = memory.ToArray();

        var documento = new Documento
        {
            SolicitacaoProgressaoId = model.SolicitacaoProgressaoId,
            AtividadeId = model.AtividadeId,
            TipoDocumentoId = tipoDocumentoId.Value,
            NomeArquivo = Path.GetFileName(model.Arquivo.FileName),
            ContentType = model.Arquivo.ContentType,
            TamanhoBytes = model.Arquivo.Length,
            DataUpload = DateTime.UtcNow,
            OrigemDocumento = model.OrigemDocumento,
            HashSha256 = CalcularHash(bytes),
            Observacao = string.IsNullOrWhiteSpace(model.Observacao) ? null : model.Observacao.Trim(),
            Arquivo = bytes
        };

        await _documentoRepository.AdicionarAsync(documento);
        await ReabrirAtividadeComprovadaSeNecessarioAsync(atividadeRelacionada);
        solicitacao.DataUltimaMovimentacao = DateTime.UtcNow;
        await _documentoRepository.SalvarAlteracoesAsync();

        return UploadDocumentoResultadoDto.Criado(documento.DocumentoId);
    }

    public async Task<DocumentoDownloadDto?> ObterDownloadAsync(int usuarioId, int documentoId)
    {
        var documento = await _documentoRepository.ObterPorIdEUsuarioAsync(documentoId, usuarioId, incluirArquivo: true);

        if (documento == null)
            return null;

        return new DocumentoDownloadDto
        {
            NomeArquivo = documento.NomeArquivo,
            ContentType = documento.ContentType,
            Arquivo = documento.Arquivo
        };
    }

    public async Task<bool> ExcluirAsync(int usuarioId, int documentoId)
    {
        var documento = await _documentoRepository.ObterPorIdEUsuarioAsync(documentoId, usuarioId, incluirArquivo: true);

        if (documento == null)
            return false;

        var solicitacao = await _solicitacaoRepository.ObterParaEdicaoPorIdEUsuarioAsync(documento.SolicitacaoProgressaoId, usuarioId);

        if (solicitacao == null || !SolicitacaoEditavel(solicitacao))
            return false;

        _documentoRepository.Remover(documento);
        solicitacao.DataUltimaMovimentacao = DateTime.UtcNow;
        await _documentoRepository.SalvarAlteracoesAsync();

        return true;
    }

    private static bool ArquivoPdf(IFormFile arquivo)
    {
        var extensao = Path.GetExtension(arquivo.FileName);
        return string.Equals(extensao, ".pdf", StringComparison.OrdinalIgnoreCase)
            && string.Equals(arquivo.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase);
    }

    private static string CalcularHash(byte[] bytes)
    {
        return Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    }

    private static bool SolicitacaoEditavel(SolicitacaoProgressao solicitacao)
    {
        return StatusSolicitacaoNomes.PermiteEdicaoProfessor(solicitacao.StatusSolicitacao.Nome);
    }

    private async Task<int?> ResolverTipoDocumentoIdAsync(DocumentoUploadViewModel model)
    {
        if (model.OrigemDocumento == OrigensDocumento.ComprovatorioProfessor)
            return (await _documentoRepository.ObterTipoDocumentoComprovanteAtividadeAsync())?.TipoDocumentoId;

        if (!model.TipoDocumentoId.HasValue)
            return null;

        return await _documentoRepository.TipoDocumentoExisteAsync(model.TipoDocumentoId.Value)
            ? model.TipoDocumentoId.Value
            : null;
    }

    private async Task ReabrirAtividadeComprovadaSeNecessarioAsync(Atividade? atividade)
    {
        if (atividade?.SolicitacaoProgressao == null)
            return;

        atividade.Status = "Declarada";
        atividade.ParecerAvaliacao = null;
        atividade.DataAvaliacao = null;

        if (atividade.SolicitacaoProgressao.StatusSolicitacao.Nome != StatusSolicitacaoNomes.AjustesSolicitados)
            return;

        var statusEmProcessamento = await _solicitacaoRepository.ObterStatusPorNomeAsync(StatusSolicitacaoNomes.EmProcessamento);
        if (statusEmProcessamento == null)
            return;

        atividade.SolicitacaoProgressao.StatusSolicitacaoId = statusEmProcessamento.StatusSolicitacaoId;
        atividade.SolicitacaoProgressao.DataFechamento = null;
    }

    private static DocumentoResumoDto MapearResumo(Documento documento)
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
}
