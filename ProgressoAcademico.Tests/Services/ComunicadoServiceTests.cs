using FluentAssertions;
using Moq;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Comunicados;
using ProgressoAcademico.Services.Comunicados;

namespace ProgressoAcademico.Tests.Services;

public class ComunicadoServiceTests
{
    [Fact]
    public async Task AtualizarAsync_ComComunicadoExistente_DeveAtualizarConteudoEData()
    {
        var comunicado = new Comunicado
        {
            ComunicadoId = 7,
            Titulo = "Titulo anterior",
            Resumo = "Resumo anterior",
            Conteudo = "Conteudo anterior",
            Categoria = "Informativo",
            Publicado = true
        };

        var repository = new Mock<IComunicadoRepository>();
        repository.Setup(r => r.ObterPorIdAsync(7)).ReturnsAsync(comunicado);
        var service = new ComunicadoService(repository.Object);

        var resultado = await service.AtualizarAsync(new ComunicadoFormViewModel
        {
            ComunicadoId = 7,
            Titulo = "  Novo titulo  ",
            Resumo = "  Novo resumo  ",
            Conteudo = "  Novo conteudo  ",
            Categoria = "  Prazo  ",
            Publicado = false
        });

        resultado.Should().BeTrue();
        comunicado.Titulo.Should().Be("Novo titulo");
        comunicado.Resumo.Should().Be("Novo resumo");
        comunicado.Conteudo.Should().Be("Novo conteudo");
        comunicado.Categoria.Should().Be("Prazo");
        comunicado.Publicado.Should().BeFalse();
        comunicado.DataAtualizacao.Should().NotBeNull();
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterParaEdicaoAsync_ComIdInexistente_DeveRetornarNulo()
    {
        var repository = new Mock<IComunicadoRepository>();
        repository.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Comunicado?)null);
        var service = new ComunicadoService(repository.Object);

        var resultado = await service.ObterParaEdicaoAsync(99);

        resultado.Should().BeNull();
    }
}
