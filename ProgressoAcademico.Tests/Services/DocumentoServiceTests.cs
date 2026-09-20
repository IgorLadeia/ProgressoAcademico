using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Documentos;
using ProgressoAcademico.Services.Documentos;
using System.Security.Cryptography;

namespace ProgressoAcademico.Tests.Services;

public class DocumentoServiceTests
{
    [Fact]
    public async Task EnviarAsync_ComPdfValido_DeveSalvarDocumentoComHash()
    {
        Documento? salvo = null;
        var solicitacao = TestDataFactory.Solicitacao();
        var bytes = new byte[] { 37, 80, 68, 70, 45, 49, 46, 52 };

        var documentoRepository = new Mock<IDocumentoRepository>();
        documentoRepository.Setup(r => r.ObterTipoDocumentoComprovanteAtividadeAsync())
            .ReturnsAsync(TestDataFactory.TipoDocumento());
        documentoRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Documento>()))
            .Callback<Documento>(d =>
            {
                d.DocumentoId = 44;
                salvo = d;
            })
            .Returns(Task.CompletedTask);

        var solicitacaoRepository = new Mock<ISolicitacaoProgressaoRepository>();
        solicitacaoRepository
            .Setup(r => r.ObterParaEdicaoPorIdEUsuarioAsync(solicitacao.SolicitacaoProgressaoId, solicitacao.UsuarioId))
            .ReturnsAsync(solicitacao);

        var atividadeRepository = new Mock<IAtividadeRepository>();
        atividadeRepository.Setup(r => r.ObterPorIdEUsuarioAsync(10, solicitacao.UsuarioId))
            .ReturnsAsync(new Atividade { AtividadeId = 10, SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId });

        var service = new DocumentoService(documentoRepository.Object, solicitacaoRepository.Object, atividadeRepository.Object);

        var resultado = await service.EnviarAsync(solicitacao.UsuarioId, new DocumentoUploadViewModel
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            AtividadeId = 10,
            OrigemDocumento = OrigensDocumento.ComprovatorioProfessor,
            Observacao = "  Evidencia de ensino  ",
            Arquivo = CriarArquivo(bytes, "comprovante.pdf", "application/pdf")
        });

        resultado.Sucesso.Should().BeTrue();
        resultado.DocumentoId.Should().Be(44);
        salvo.Should().NotBeNull();
        salvo!.NomeArquivo.Should().Be("comprovante.pdf");
        salvo.Observacao.Should().Be("Evidencia de ensino");
        salvo.HashSha256.Should().Be(Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant());
        salvo.Arquivo.Should().Equal(bytes);
        documentoRepository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task EnviarAsync_ComArquivoNaoPdf_DeveBloquearUpload()
    {
        var solicitacao = TestDataFactory.Solicitacao();
        var documentoRepository = new Mock<IDocumentoRepository>();
        documentoRepository.Setup(r => r.ObterTipoDocumentoComprovanteAtividadeAsync())
            .ReturnsAsync(TestDataFactory.TipoDocumento());

        var solicitacaoRepository = new Mock<ISolicitacaoProgressaoRepository>();
        solicitacaoRepository
            .Setup(r => r.ObterParaEdicaoPorIdEUsuarioAsync(solicitacao.SolicitacaoProgressaoId, solicitacao.UsuarioId))
            .ReturnsAsync(solicitacao);

        var atividadeRepository = new Mock<IAtividadeRepository>();
        atividadeRepository.Setup(r => r.ObterPorIdEUsuarioAsync(10, solicitacao.UsuarioId))
            .ReturnsAsync(new Atividade { AtividadeId = 10, SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId });

        var service = new DocumentoService(documentoRepository.Object, solicitacaoRepository.Object, atividadeRepository.Object);

        var resultado = await service.EnviarAsync(solicitacao.UsuarioId, new DocumentoUploadViewModel
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            AtividadeId = 10,
            OrigemDocumento = OrigensDocumento.ComprovatorioProfessor,
            Arquivo = CriarArquivo(new byte[] { 1, 2, 3 }, "imagem.png", "image/png")
        });

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Contain("PDF");
        documentoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Documento>()), Times.Never);
        documentoRepository.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task EnviarAsync_ComSolicitacaoFinalizada_DeveBloquearNovoAnexo()
    {
        var solicitacao = TestDataFactory.Solicitacao(statusNome: StatusSolicitacaoNomes.Encerrada);
        var documentoRepository = new Mock<IDocumentoRepository>();

        var solicitacaoRepository = new Mock<ISolicitacaoProgressaoRepository>();
        solicitacaoRepository
            .Setup(r => r.ObterParaEdicaoPorIdEUsuarioAsync(solicitacao.SolicitacaoProgressaoId, solicitacao.UsuarioId))
            .ReturnsAsync(solicitacao);

        var atividadeRepository = new Mock<IAtividadeRepository>();
        var service = new DocumentoService(documentoRepository.Object, solicitacaoRepository.Object, atividadeRepository.Object);

        var resultado = await service.EnviarAsync(solicitacao.UsuarioId, new DocumentoUploadViewModel
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            TipoDocumentoId = 1,
            OrigemDocumento = OrigensDocumento.ComprovatorioProfessor,
            Arquivo = CriarArquivo(new byte[] { 1, 2, 3 }, "comprovante.pdf", "application/pdf")
        });

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Contain("finalizadas");
        documentoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Documento>()), Times.Never);
    }

    private static IFormFile CriarArquivo(byte[] bytes, string nome, string contentType)
    {
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "arquivo", nome)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}
